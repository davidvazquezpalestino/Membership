using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        ClaimsIdentity Identity = new ClaimsIdentity();

        var StoredTokens = await GetUserTokensAsync();
        if(StoredTokens != null )
        {
            var Token = new JwtSecurityTokenHandler()
                .ReadJwtToken(StoredTokens.AccessToken);

            Identity = new ClaimsIdentity(Token.Claims, "Bearer");
        }
        return new AuthenticationState(new ClaimsPrincipal(Identity));
    }

    public async Task<UserTokensDto> GetUserTokensAsync()
    {
        UserTokensDto StoredTokens = await TokensRepository.GetTokensAsync();
        if(StoredTokens != null)
        {
            var Token = new JwtSecurityTokenHandler()
                .ReadJwtToken(StoredTokens.AccessToken);

            if (Token.ValidTo <= DateTime.UtcNow)
            {
                try
                {
                    var NewTokens = await UserWebApiGateway
                        .RefreshTokenAsync(StoredTokens);
                    await LoginAsync(NewTokens);
                    StoredTokens = NewTokens;
                    Console.WriteLine("Access token actualizado");
                }
                catch(Exception ex)
                {
                    StoredTokens = default;
                    Console.WriteLine(ex.Message);
                    await LogoutAsync();
                }
            }
        }
        return StoredTokens;
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

