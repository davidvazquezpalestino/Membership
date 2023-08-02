using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var Response = await TokenService.GetTokensAsync(State, Code);
            string Action = Response.Scope[..Response.Scope.IndexOf("_")];
            if (Enum.TryParse<ScopeAction>(Action, out ScopeAction ScopeAction))
            {
                switch (ScopeAction)
                {
                    case ScopeAction.Register:
                    case ScopeAction.Login:
                        await AuthenticationStateProvider.LoginAsync(
                            Response.Tokens);
                        break;
                }
            }
            if (string.IsNullOrWhiteSpace(Response.ReturnUri))
            {
                NavigationManager.NavigateTo("");
            }
            else
            {
                NavigationManager.NavigateTo(Response.ReturnUri);
            }
        }
        catch (HttpRequestException ex)
        {
            IEnumerable<MembershipError> Errors = null;
            if (ex.Data.Contains("Errors"))
            {
                Errors = ex.Data["Errors"] as IEnumerable<MembershipError>;
            }
            OnError(string.IsNullOrWhiteSpace(ex.Message) ?
                Localizer[MessageKeys.UnableToGetExternalUserTokens] : ex.Message
                , Errors);
        }
        catch (Exception ex)
        {
            OnError(string.IsNullOrWhiteSpace(ex.Message) ?
                Localizer[MessageKeys.UnableToGetExternalUserTokens] : ex.Message
                , null);
        }
    }

    protected virtual void OnError(string message,
        IEnumerable<MembershipError> errors)
    { }
}
