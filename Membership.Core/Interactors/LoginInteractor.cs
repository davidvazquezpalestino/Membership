﻿namespace Membership.Core.Interactors;
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

    public async Task LogingAsync(UserCredentialsDto userCredentialsDto)
    {
        IEnumerable<MembershipError> validationErrors = Validator.Validate(userCredentialsDto);
        if (validationErrors != null && validationErrors.Any())
        {
            throw new LoginUserException();
        }

        UserEntity user = await UserManagerService.
            ThrowIfUnableToGetUserByCredentialsAsync(userCredentialsDto);

        await OutputPort.HandleUserEntityAsync(user);
    }
}
