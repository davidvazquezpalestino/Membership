namespace Membership.UserManagerService.AspNetIdentity.Services;
internal partial class UserManagerService
{
    public async Task<IEnumerable<MembershipError>> RegisterExternalUserAsync(
        ExternalUserEntity user)
    {
        IEnumerable<MembershipError> errors = null;
        IdentityResult result = null;
        bool userExists = false;

        User existingUser = await UserManager.FindByEmailAsync(user.Email);
        if (existingUser == null)
        {
            existingUser = new User
            {
                UserName = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            result = await UserManager.CreateAsync(existingUser);
            userExists = result.Succeeded;
        }
        else
        {
            userExists = true;
        }

        if (userExists)
        {
            result = await UserManager.AddLoginAsync(existingUser,
                new UserLoginInfo(user.LoginProvider, user.ProviderUserId,
                user.LoginProvider));
        }

        if (!result.Succeeded)
        {
            errors = ErrorsHandler.Handle(result.Errors);
        }

        return errors;
    }

    public async Task<UserEntity> GetUserByExternalCredentialsAsync(
        ExternalUserCredentials userCredentials)
    {
        UserEntity foundUser = default;
        User user = await UserManager.FindByLoginAsync(
            userCredentials.LoginProvider, userCredentials.ProviderUserId);
        if (user != null)
        {
            foundUser = new UserEntity(user.UserName,
                user.FirstName, user.LastName);
        }
        return foundUser;
    }
}



