namespace Membership.Blazor.Components;

public partial class LoginComponent
{
    [Inject]
    IUserWebApiGateway Gateway { get; set; }
    [Inject]
    IAuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject]
    IValidator<UserCredentialsDto> UserCredentialsValidator { get; set; }
    [Inject]
    IMembershipMessageLocalizer Localizer { get; set; }

    [Parameter]
    public EventCallback<UserTokensDto> OnLogin { get; set; }

    UserCredentialsDto UserCredentials = new();
    string ErrorMessage;

    async Task Login()
    {
        try
        {
            ErrorMessage = null;
            var Tokens = await Gateway.LoginAsync(UserCredentials);
            await AuthenticationStateProvider.LoginAsync(Tokens);
            await OnLogin.InvokeAsync(Tokens);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch
        {
            throw;
        }
    }
}