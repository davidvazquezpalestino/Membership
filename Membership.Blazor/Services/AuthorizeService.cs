using Membership.Shared.Interfaces;
using Membership.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Blazor.Services;
internal class AuthorizeService: IAuthorizeService
{
    readonly IOptions<AppOptions> AppOptions;
    readonly IOAuthStateService StateService;
    readonly IOAuthService OAuthService;
    readonly NavigationManager NavigationManager;

    public AuthorizeService(IOptions<AppOptions> appOptions, 
        IOAuthStateService stateService, IOAuthService oAuthService, 
        NavigationManager navigationManager)
    {
        AppOptions = appOptions;
        StateService = stateService;
        OAuthService = oAuthService;
        NavigationManager = navigationManager;
    }

    public ExternalIDPInfo[] IDPs => AppOptions.Value.IDPs;

    public async Task AuthorizeAsync(string providerId, ScopeAction action,
        string returnUri)
    {
        var StateInfo = new StateInfo(
            OAuthService.GetState(), OAuthService.GetCodeVerifier(),
            OAuthService.GetNonce(), $"{action}_{providerId}", returnUri);

        var RequestData = new AuthorizeRequestInfo(
            AppOptions.Value.AuthorizationEndpoint,
            AppOptions.Value.ClientId,
            AppOptions.Value.RedirectUri,
            StateInfo.State,
            StateInfo.Scope,
            OAuthService.GetHash256CodeChallenge(StateInfo.CodeVerifier),
            OAuthService.CodeChallengeMethodSha256,
            StateInfo.Nonce);

        await StateService.SetAsync(StateInfo.State, StateInfo);

        NavigationManager.NavigateTo(
            OAuthService.BuildAuthorizeRequestUri(RequestData));
    }

}
