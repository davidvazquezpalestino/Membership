namespace Membership.Core.Interactors;
internal class LoginInteractor : ILoginInputPort
{
    readonly IUserManagerService UserManagerService;
    readonly IValidator<UserCredentialsDto> Validator;
    readonly ILoginOutputPort OutputPort;

    public LoginInteractor(IUserManagerService userManagerService,
        IValidator<UserCredentialsDto> validator, ILoginOutputPort outputPort)
    {
        UserManagerService = userManagerService;
        Validator = validator;
        OutputPort = outputPort;
    }

    public async Task LogingAsync(UserCredentialsDto userCredentials)
    {
        var ValidationErrors = Validator.Validate(userCredentials);
        if (ValidationErrors != null && ValidationErrors.Any())
        {
            throw new LoginUserException();
        }

        var User = await UserManagerService.
            ThrowIfUnableToGetUserByCredentialsAsync(userCredentials);

        await OutputPort.HandleUserEntityAsync(User);
    }
}
