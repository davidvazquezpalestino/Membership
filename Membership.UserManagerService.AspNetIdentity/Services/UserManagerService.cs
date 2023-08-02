namespace Membership.UserManagerService.AspNetIdentity.Services;
internal partial class UserManagerService : IUserManagerService
{
    readonly UserManagerErrorsHandler ErrorsHandler;
    readonly UserManager<User> UserManager;

    public UserManagerService(UserManagerErrorsHandler errorsHandler,
        UserManager<User> userManager)
    {
        ErrorsHandler = errorsHandler;
        UserManager = userManager;
    }
}
