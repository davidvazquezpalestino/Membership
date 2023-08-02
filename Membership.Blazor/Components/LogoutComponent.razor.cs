namespace Membership.Blazor.Components;

public partial class LogoutComponent
{
    [Inject]
    IUserWebApiGateway Gateway { get; set; }
    [Inject]
    IAuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject]
    IMembershipMessageLocalizer Localizer { get; set; }
    [Inject]
    NavigationManager NavigationManager { get; set; }

    async void Logout()
    {
        var StoredTokens =
            await AuthenticationStateProvider.GetUserTokensAsync();
        if (StoredTokens != null)
        {
            await Gateway.LogoutAsync(StoredTokens);
        }
        await AuthenticationStateProvider.LogoutAsync();
        NavigationManager.NavigateTo("");
    }
}