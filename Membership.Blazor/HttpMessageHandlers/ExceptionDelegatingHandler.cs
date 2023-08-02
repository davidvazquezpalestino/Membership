using Membership.Shared.ValueObjects;

namespace Membership.Blazor.HttpMessageHandlers;
internal class ExceptionDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var Response = await base.SendAsync(request, cancellationToken);
        if (!Response.IsSuccessStatusCode)
        {
            string ErrorMessage = await Response.Content.ReadAsStringAsync();
            string Source = null;
            string Message = null;
            IEnumerable<MembershipError> Errors = null;
            bool IsValidProblemDetails = false;

            try
            {
                var JsonResponse = JsonSerializer.Deserialize<JsonElement>(ErrorMessage);
                if (JsonResponse.TryGetProperty("instance", out JsonElement InstanceValue))
                {
                    string Value = InstanceValue.ToString();
                    if (Value.ToLower().StartsWith("problemdetails"))
                    {
                        Source = Value;
                        if (JsonResponse.TryGetProperty("title", out var TitleValue))
                            Message = TitleValue.ToString();
                        if (JsonResponse.TryGetProperty("errors",
                            out JsonElement ErrorsValue))
                        {
                            Errors = JsonSerializer
                                .Deserialize<IEnumerable<MembershipError>>(ErrorsValue);
                        }
                        IsValidProblemDetails = true;
                    }
                }
            }
            catch { }
            if (!IsValidProblemDetails)
            {
                Message = ErrorMessage;
                Source = null;
                Errors = null;
            }
            var Ex = new HttpRequestException(Message, null, Response.StatusCode);
            Ex.Source = Source;
            if (Errors != null) Ex.Data.Add("Errors", Errors);
            throw Ex;
        }
        return Response;
    }
}
