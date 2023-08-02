using Membership.Shared.ValueObjects;

namespace Membership.UserManagerService.AspNetIdentity.Services;
internal partial class UserManagerService
{
    public async Task<IEnumerable<MembershipError>> RegisterExternalUserAsync(
        ExternalUserEntity user)
    {
        IEnumerable<MembershipError> Errors = null;
        IdentityResult Result = null;
        bool UserExists = false;

        var ExistingUser = await UserManager.FindByEmailAsync(user.Email);
        if (ExistingUser == null)
        {
            ExistingUser = new User
            {
                UserName = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            Result = await UserManager.CreateAsync(ExistingUser);
            UserExists = Result.Succeeded;
        }
        else
        {
            UserExists = true;
        }

        if (UserExists)
        {
            Result = await UserManager.AddLoginAsync(ExistingUser,
                new UserLoginInfo(user.LoginProvider, user.ProviderUserId,
                user.LoginProvider));
        }

        if (Result != null && !Result.Succeeded)
        {
            Errors = ErrorsHandler.Handle(Result.Errors);
        }

        return Errors;
    }

    public async Task<UserEntity> GetUserByExternalCredentialsAsync(
        ExternalUserCredentials userCredentials)
    {
        UserEntity FoundUser = default;
        var User = await UserManager.FindByLoginAsync(
            userCredentials.LoginProvider, userCredentials.ProviderUserId);
        if (User != null)
        {
            FoundUser = new UserEntity(User.UserName,
                User.FirstName, User.LastName);
        }
        return FoundUser;
    }
}



