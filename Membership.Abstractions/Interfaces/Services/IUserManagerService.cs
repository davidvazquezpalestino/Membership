namespace Membership.Abstractions.Interfaces.Services;
public interface IUserManagerService
{
    Task<IEnumerable<MembershipError>> RegisterUserAsync(UserDto userDto);
    Task<IEnumerable<MembershipError>> RegisterExternalUserAsync(ExternalUserEntity user);

    Task<UserEntity> GetUserByCredentialsAsync(UserCredentialsDto userCredentials);
    Task<UserEntity> GetUserByExternalCredentialsAsync(ExternalUserCredentials userCredentials);

    async Task ThrowIfUnableToRegisterUserAsync(UserDto user)
    {
        IEnumerable<MembershipError> errors = await RegisterUserAsync(user);
        if (errors != null && errors.Any())
        {
            throw new RegisterUserException(errors);
        }
    }
    async Task ThrowIfUnableToRegisterExternalUserAsync(ExternalUserEntity user)
    {
        IEnumerable<MembershipError> errors = await RegisterExternalUserAsync(user);
        if (errors != null && errors.Any())
        {
            throw new RegisterUserException(errors);
        }
    }

    async Task<UserEntity> ThrowIfUnableToGetUserByCredentialsAsync(UserCredentialsDto userCredentials)
    {
        UserEntity user = await GetUserByCredentialsAsync(userCredentials);
        if (user == default)
        {
            throw new LoginUserException();
        }

        return user;
    }
    async Task<UserEntity> ThrowIfUnableToGetUserByExternalCredentialsAsync(ExternalUserCredentials userCredentials)
    {
        UserEntity user = await GetUserByExternalCredentialsAsync(userCredentials);
        if (user == default)
        {
            throw new LoginUserException();
        }

        return user;
    }
}
