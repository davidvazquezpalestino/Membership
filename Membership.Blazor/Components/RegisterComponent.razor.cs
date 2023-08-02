namespace Membership.Blazor.Components;
public partial class RegisterComponent
{
    [Inject]
    IUserWebApiGateway Gateway { get; set; }
    [Inject]
    IValidator<UserDto> UserValidator { get; set; }
    [Inject]
    IMembershipMessageLocalizer Localizer { get; set; }

    [Parameter]
    public EventCallback<UserDto> OnRegister { get; set; }

    MembershipValidator<UserDto> MembershipValidator;

    UserDto User = new();
    async Task Register()
    {
        try
        {
            await Gateway.RegisterUserAsync(User);
            await OnRegister.InvokeAsync(User);
        }
        catch (HttpRequestException ex)
        {
            MembershipValidator.TrySetErrorsFromHttpRequestException(ex);
        }
        catch
        {
            throw;
        }
    }
}
