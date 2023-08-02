using Membership.Shared.ValueObjects;

namespace Membership.UserManagerService.AspNetIdentity.Services;
internal class UserManagerErrorsHandler
{
    readonly IMembershipMessageLocalizer Localizer;

    public UserManagerErrorsHandler(IMembershipMessageLocalizer localizer)
    {
        Localizer = localizer;
    }

    public IEnumerable<MembershipError> Handle(IEnumerable<IdentityError> errors)
    {
        List<MembershipError> Errors = new();
        foreach (var Error in errors)
        {
            switch (Error.Code)
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
                    Errors.Add(new(Error.Code, Error.Description));
                    break;

            }
        }
        return Errors;
    }
}
