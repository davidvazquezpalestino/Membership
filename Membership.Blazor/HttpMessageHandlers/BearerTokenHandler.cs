namespace Membership.Blazor.HttpMessageHandlers;
internal class BearerTokenHandler : DelegatingHandler
{
    readonly IAuthenticationStateProvider Provider;

    public BearerTokenHandler(IAuthenticationStateProvider provider) => Provider = provider;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        UserTokensDto storedTokens = await Provider.GetUserTokensAsync();
        if (storedTokens != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedTokens.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
