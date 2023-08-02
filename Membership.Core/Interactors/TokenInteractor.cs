using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Core.Interactors;
internal class TokenInteractor : ITokenInputPort
{
    readonly IOAuthStateService OAuthStateService;
    readonly IOAuthService OAuthService;
    readonly IUserManagerService UserManagerService;
    readonly ILoginOutputPort Presenter;

    public TokenInteractor(IOAuthStateService oAuthStateService, 
        IOAuthService oAuthService,
        IUserManagerService userManagerService, ILoginOutputPort presenter)
    {
        OAuthStateService = oAuthStateService;
        OAuthService = oAuthService;
        UserManagerService = userManagerService;
        Presenter = presenter;
    }

    public async Task HandleTokenRequestAsync(TokenRequestInfo info)
    {
        UserEntity UserEntity = null;
        StateInfo StateInfo = await OAuthStateService.GetAsync<StateInfo>(info.Code);
        if (StateInfo == null)
            throw new InvalidAuthorizationCodeException();
        if(StateInfo.AppClientStateInfo.RedirectUri != info.RedirectUri)
            throw new InvalidRedirectUriException();
        if(StateInfo.AppClientStateInfo.ClientId != info.ClientId)
            throw new InvalidClientIdException();
        if(StateInfo.AppClientStateInfo.Scope != info.Scope)
            throw new InvalidScopeException();
        if(StateInfo.AppClientStateInfo.CodeChallenge !=
            OAuthService.GetHash256CodeChallenge(info.CodeVerifier))
            throw new InvalidCodeVerifierException();

        string Action = info.Scope[..info.Scope.IndexOf("_")]?.ToLower();

        var IdentityToken = new JwtSecurityTokenHandler()
            .ReadJwtToken(StateInfo.Tokens.IdToken);

        string FirstName = IdentityToken.Claims.FirstOrDefault(
            c => c.Type == "given_name")?.Value;
        string LastName = IdentityToken.Claims.FirstOrDefault(
            c => c.Type == "family_name")?.Value;
        string Name = IdentityToken.Claims.FirstOrDefault(
            c => c.Type == "name")?.Value;
        string Sub = IdentityToken.Claims.FirstOrDefault(
            c => c.Type == "sub")?.Value;

        string Email = IdentityToken.Claims.FirstOrDefault(
            c => c.Type == "email")?.Value;

        if (string.IsNullOrEmpty(Email)) Email = Sub;
        if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
            FirstName = Name ?? Email;

        var ExternalUserEntity = new ExternalUserEntity(Email, FirstName,
            LastName, StateInfo.ProviderId, Sub);

        switch(Action)
        {
            case "register":
                await UserManagerService.ThrowIfUnableToRegisterExternalUserAsync
                    (ExternalUserEntity);
                UserEntity = await UserManagerService
                    .ThrowIfUnableToGetUserByExternalCredentialsAsync(
                    new ExternalUserCredentials(
                        ExternalUserEntity.LoginProvider,
                        ExternalUserEntity.ProviderUserId));
                break;
            case "login":
                UserEntity = await UserManagerService
                    .ThrowIfUnableToGetUserByExternalCredentialsAsync(
                    new ExternalUserCredentials(
                        ExternalUserEntity.LoginProvider,
                        ExternalUserEntity.ProviderUserId));
                break;
            default:
                throw new InvalidScopeActionException();
        }
        UserEntity.Claims = new List<Claim> { new Claim("nonce", 
            StateInfo.AppClientStateInfo.Nonce) };
        await Presenter.HandleUserEntityAsync(UserEntity);
    }
}

