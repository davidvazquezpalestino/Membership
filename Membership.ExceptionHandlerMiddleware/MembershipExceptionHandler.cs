namespace Membership.ExceptionHandlerMiddleware;
public static class MembershipExceptionHandler
{
    static Dictionary<Type, Delegate> ExceptionHandlers = new();
    public static void AddHandler(Type exceptionType, Delegate @delegate) =>
        ExceptionHandlers.TryAdd(exceptionType, @delegate);

    public static void AddHttp400Handler<T>()
    {
        string ExceptionTypeName = typeof(T).Name;
        ExceptionHandlers.TryAdd(typeof(T),
            (T ex, IMembershipMessageLocalizer localizer) =>
            new ProblemDetails().FromHttp400BadRequest(
                localizer[$"{ExceptionTypeName}Message"],
                ExceptionTypeName));
    }

    public static async Task<bool> WriteResponse(
        HttpContext context, IMembershipMessageLocalizer localizer)
    {
        IExceptionHandlerFeature ExceptionDetail =
            context.Features.Get<IExceptionHandlerFeature>();

        Exception ExceptionError = ExceptionDetail?.Error;

        bool Handled = true;

        if (ExceptionError != null)
        {
            if (ExceptionHandlers.TryGetValue(ExceptionError.GetType(),
                out Delegate Handler))
            {
                await WriteProblemDetailsAsync(context,
                    Handler.DynamicInvoke(ExceptionError, localizer)
                    as ProblemDetails);
            }
            else
            {
                Handled = false;
            }
        }
        return Handled;
    }


    public static ProblemDetails FromHttp400BadRequest(
        this ProblemDetails problem,
        string title, string instance, object extensions = null)
    {
        problem.Status = StatusCodes.Status400BadRequest;
        problem.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
        problem.Title = title;
        problem.Instance = $"problemDetails/{instance}";
        if (extensions != null)
            problem.Extensions.Add("errors", extensions);
        return problem;
    }

    static async Task WriteProblemDetailsAsync(HttpContext context,
        ProblemDetails details)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = details.Status.Value;
        var Stream = context.Response.Body;
        await JsonSerializer.SerializeAsync(Stream, details);
    }

}
