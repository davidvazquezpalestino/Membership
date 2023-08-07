namespace Membership.Blazor.Components;
public class ExternalCallbackEndpointBase : ComponentBase
{
    [Inject]
    TokenService TokenService { get; set; }

    [Inject]
    IAuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    NavigationManager NavigationManager { get; set; }

    [Inject]
    IMembershipMessageLocalizer Localizer { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string Code { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string State { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            TokenServiceResponse response = await TokenService.GetTokensAsync(State, Code);
            string action = response.Scope[..response.Scope.IndexOf("_")];
            
            if (Enum.TryParse<ScopeAction>(action, out ScopeAction scopeAction))
            {
                switch (scopeAction)
                {
                    case ScopeAction.Register:
                    case ScopeAction.Login:
                        await AuthenticationStateProvider.LoginAsync(response.Tokens);
                        break;
                }
            }

            NavigationManager.NavigateTo(string.IsNullOrWhiteSpace(response.ReturnUri) ? "" : response.ReturnUri);
        }
        catch (HttpRequestException ex)
        {
            IEnumerable<MembershipError> errors = null;
            if (ex.Data.Contains("Errors"))
            {
                errors = ex.Data["Errors"] as IEnumerable<MembershipError>;
            }
            OnError(string.IsNullOrWhiteSpace(ex.Message) ?
                Localizer[MessageKeys.UnableToGetExternalUserTokens] : ex.Message
                , errors);
        }
        catch (Exception ex)
        {
            OnError(string.IsNullOrWhiteSpace(ex.Message) ?
                Localizer[MessageKeys.UnableToGetExternalUserTokens] : ex.Message
                , null);
        }
    }

    protected virtual void OnError(string message, IEnumerable<MembershipError> errors){ }
}
