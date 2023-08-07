using System.Security.Claims;

namespace Membership.Blazor.AuthenticationStateProviders;
internal class JWTAuthenticationStateProvider : AuthenticationStateProvider,
    IAuthenticationStateProvider
{
    readonly IUserWebApiGateway UserWebApiGateway;
    readonly ITokensRepository TokensRepository;

    public JWTAuthenticationStateProvider(IUserWebApiGateway userWebApiGateway, 
        ITokensRepository tokensRepository)
    {
        UserWebApiGateway = userWebApiGateway;
        TokensRepository = tokensRepository;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();

        UserTokensDto storedTokens = await GetUserTokensAsync();
        if(storedTokens != null )
        {
            JwtSecurityToken token = new JwtSecurityTokenHandler()
                .ReadJwtToken(storedTokens.AccessToken);

            claimsIdentity = new ClaimsIdentity(token.Claims, "Bearer");
        }
        return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
    }

    public async Task<UserTokensDto> GetUserTokensAsync()
    {
        UserTokensDto storedTokens = await TokensRepository.GetTokensAsync();
        if(storedTokens != null)
        {
            JwtSecurityToken token = new JwtSecurityTokenHandler()
                .ReadJwtToken(storedTokens.AccessToken);

            if (token.ValidTo <= DateTime.UtcNow)
            {
                try
                {
                    UserTokensDto newTokens = await UserWebApiGateway
                        .RefreshTokenAsync(storedTokens);
                    await LoginAsync(newTokens);
                    storedTokens = newTokens;
                    Console.WriteLine("Access token actualizado");
                }
                catch(Exception ex)
                {
                    storedTokens = default;
                    Console.WriteLine(ex.Message);
                    await LogoutAsync();
                }
            }
        }
        return storedTokens;
    }

    public async Task LoginAsync(UserTokensDto userTokensDto)
    {
        await TokensRepository.SaveTokensAsync(userTokensDto);
        NotifyAuthenticationStateChanged(
            GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        await TokensRepository.RemoveTokensAsync();
        NotifyAuthenticationStateChanged( GetAuthenticationStateAsync());
    }
}

