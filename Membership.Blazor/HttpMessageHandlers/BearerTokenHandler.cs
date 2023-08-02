namespace Membership.Blazor.HttpMessageHandlers;
internal class BearerTokenHandler : DelegatingHandler
{
    readonly IAuthenticationStateProvider Provider;

    public BearerTokenHandler(IAuthenticationStateProvider provider)
    {
        Provider = provider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var StoredTokens = await Provider.GetUserTokensAsync();
        if (StoredTokens != null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", StoredTokens.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
