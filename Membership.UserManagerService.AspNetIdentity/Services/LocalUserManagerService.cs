namespace Membership.UserManagerService.AspNetIdentity.Services;
internal partial class UserManagerService
{
    public async Task<IEnumerable<MembershipError>> RegisterUserAsync(UserDto userDto)
    {
        IEnumerable<MembershipError> errors = null;

        User user = new User
        {
            UserName = userDto.Email,
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName
        };

        IdentityResult result = await UserManager.CreateAsync(user, userDto.Password);
        if (!result.Succeeded)
        {
            errors = ErrorsHandler.Handle(result.Errors);
        }

        return errors;
    }

    public async Task<UserEntity> GetUserByCredentialsAsync(UserCredentialsDto userCredentials)
    {
        UserEntity foundUser = default;
        User user = await UserManager.FindByNameAsync(userCredentials.Email);
        if (user != null && await UserManager.CheckPasswordAsync(user, userCredentials.Password))
        {
            foundUser = new UserEntity(user.UserName, user.FirstName, user.LastName);
        }
        return foundUser;
    }
}
