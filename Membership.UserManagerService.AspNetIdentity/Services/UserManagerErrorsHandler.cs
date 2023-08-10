namespace Membership.UserManagerService.AspNetIdentity.Services;
internal class UserManagerErrorsHandler
{
    readonly IMembershipMessageLocalizer Localizer;
    public UserManagerErrorsHandler(IMembershipMessageLocalizer localizer) => Localizer = localizer;

    public IEnumerable<MembershipError> Handle(IEnumerable<IdentityError> errors)
    {
        List<MembershipError> membershipErrors = new ();
        foreach (IdentityError identityError in errors)
        {
            switch (identityError.Code)
            {
                case nameof(IdentityErrorDescriber.DuplicateUserName):
                    membershipErrors.Add(new MembershipError(nameof(User.Email),
                        Localizer[MessageKeys.DuplicateEmailErrorMessage]));
                    break;
                case nameof(IdentityErrorDescriber.LoginAlreadyAssociated):
                    membershipErrors.Add(new MembershipError(nameof(User.Email),
                        Localizer[MessageKeys.LoginAlreadyAssociatedErrorMessage]));
                    break;
                default:
                    membershipErrors.Add(new MembershipError(identityError.Code, identityError.Description));
                    break;
            }
        }
        return membershipErrors;
    }
}
