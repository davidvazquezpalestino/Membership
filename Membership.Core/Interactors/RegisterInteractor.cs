namespace Membership.Core.Interactors;
internal class RegisterInteractor : IRegisterInputPort
{
    readonly IUserManagerService UserManagerService;
    readonly IValidator<UserDto> Validator;

    public RegisterInteractor(IUserManagerService userManagerService, IValidator<UserDto> validator)
    {
        UserManagerService = userManagerService;
        Validator = validator;
    }

    public async Task RegisterAsync(UserDto user)
    {
        IEnumerable<MembershipError> validationErrors = Validator.Validate(user);
        if (validationErrors != null && validationErrors.Any())
        {
            throw new RegisterUserException(validationErrors);
        }
        await UserManagerService.ThrowIfUnableToRegisterUserAsync(user);
    }
}
