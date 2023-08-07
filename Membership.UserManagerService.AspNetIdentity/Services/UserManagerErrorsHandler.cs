namespace Membership.UserManagerService.AspNetIdentity.Services;
internal class UserManagerErrorsHandler
{
    readonly IMembershipMessageLocalizer Localizer;
    public UserManagerErrorsHandler(IMembershipMessageLocalizer localizer) => Localizer = localizer;

    public IEnumerable<MembershipError> Handle(IEnumerable<IdentityError> errors)
    {
        List<MembershipError> Errors = new();
        foreach (IdentityError IdentityError in errors)
        {
            switch (IdentityError.Code)
            {
                case nameof(IdentityErrorDescriber.DuplicateUserName):
                    Errors.Add(new(nameof(User.Email),
                        Localizer[MessageKeys.DuplicateEmailErrorMessage]));
                    break;
                case nameof(IdentityErrorDescriber.LoginAlreadyAssociated):
                    Errors.Add(new(nameof(User.Email),
                        Localizer[MessageKeys.LoginAlreadyAssociatedErrorMessage]));
                    break;
                default:
                    Errors.Add(new(IdentityError.Code, IdentityError.Description));
                    break;
            }
        }
        return Errors;
    }
}
