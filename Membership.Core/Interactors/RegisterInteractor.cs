namespace Membership.Core.Interactors;
internal class RegisterInteractor : IRegisterInputPort
{
    readonly IUserManagerService UserManagerService;
    readonly IValidator<UserDto> Validator;

    public RegisterInteractor(IUserManagerService userManagerService,
        IValidator<UserDto> validator)
    {
        UserManagerService = userManagerService;
        Validator = validator;
    }

    public async Task RegisterAsync(UserDto user)
    {
        var ValidationErrors = Validator.Validate(user);
        if (ValidationErrors != null && ValidationErrors.Any())
        {
            throw new RegisterUserException(ValidationErrors);
        }
        await UserManagerService.ThrowIfUnableToRegisterUserAsync(user);
    }
}
