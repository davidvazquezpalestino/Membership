﻿namespace Membership.ExceptionHandlerMiddleware;
public static class MembershipExceptionHandler
{
    static readonly Dictionary<Type, Delegate> ExceptionHandlers = new();
    public static void AddHandler(Type exceptionType, Delegate delegat) =>
        ExceptionHandlers.TryAdd(exceptionType, delegat);

    public static void AddHttp400Handler<T>()
    {
        string exceptionTypeName = typeof(T).Name;

        ExceptionHandlers.TryAdd(typeof(T), (T ex, IMembershipMessageLocalizer localizer) =>
            new ProblemDetails().FromHttp400BadRequest(localizer[$"{exceptionTypeName}Message"], exceptionTypeName));
    }

    public static async Task<bool> WriteResponse(HttpContext context, IMembershipMessageLocalizer localizer)
    {
        IExceptionHandlerFeature exceptionDetail = context.Features.Get<IExceptionHandlerFeature>();

        Exception exceptionError = exceptionDetail?.Error;

        bool handled = true;

        if (exceptionError != null)
        {
            if (ExceptionHandlers.TryGetValue(exceptionError.GetType(), out Delegate handler))
            {
                await WriteProblemDetailsAsync(context, handler.DynamicInvoke(exceptionError, localizer) as ProblemDetails);
            }
            else
            {
                handled = false;
            }
        }
        return handled;
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
        {
            problem.Extensions.Add("errors", extensions);
        }

        return problem;
    }

    static async Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails details)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = details.Status.Value;
        Stream stream = context.Response.Body;
        await JsonSerializer.SerializeAsync(stream, details);
    }

}
