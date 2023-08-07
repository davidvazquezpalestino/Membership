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
        UserEntity userEntity = null;
        StateInfo stateInfo = await OAuthStateService.GetAsync<StateInfo>(info.Code);
        if (stateInfo == null)
        {
            throw new InvalidAuthorizationCodeException();
        }

        if(stateInfo.AppClientStateInfo.RedirectUri != info.RedirectUri)
        {
            throw new InvalidRedirectUriException();
        }

        if(stateInfo.AppClientStateInfo.ClientId != info.ClientId)
        {
            throw new InvalidClientIdException();
        }

        if(stateInfo.AppClientStateInfo.Scope != info.Scope)
        {
            throw new InvalidScopeException();
        }

        if(stateInfo.AppClientStateInfo.CodeChallenge !=
           OAuthService.GetHash256CodeChallenge(info.CodeVerifier))
        {
            throw new InvalidCodeVerifierException();
        }

        string action = info.Scope[..info.Scope.IndexOf("_", StringComparison.Ordinal)]?.ToLower();

        JwtSecurityToken identityToken = new JwtSecurityTokenHandler()
            .ReadJwtToken(stateInfo.Tokens.IdToken);

        string firstName = identityToken.Claims.FirstOrDefault(claim => claim.Type == "given_name")?.Value;
        string lastName = identityToken.Claims.FirstOrDefault(claim => claim.Type == "family_name")?.Value;
        string name = identityToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
        string sub = identityToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
        string email = identityToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            email = sub;
        }

        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            firstName = name ?? email;
        }

        ExternalUserEntity externalUserEntity = new ExternalUserEntity(email, firstName,
            lastName, stateInfo.ProviderId, sub);

        switch(action)
        {
            case "register":
                await UserManagerService.ThrowIfUnableToRegisterExternalUserAsync
                    (externalUserEntity);
                userEntity = await UserManagerService
                    .ThrowIfUnableToGetUserByExternalCredentialsAsync(
                    new ExternalUserCredentials(
                        externalUserEntity.LoginProvider,
                        externalUserEntity.ProviderUserId));
                break;
            case "login":
                userEntity = await UserManagerService
                    .ThrowIfUnableToGetUserByExternalCredentialsAsync(
                    new ExternalUserCredentials(
                        externalUserEntity.LoginProvider,
                        externalUserEntity.ProviderUserId));
                break;
            default:
                throw new InvalidScopeActionException();
        }
        userEntity.Claims = new List<Claim> { new Claim("nonce", stateInfo.AppClientStateInfo.Nonce) };
        await Presenter.HandleUserEntityAsync(userEntity);
    }
}

