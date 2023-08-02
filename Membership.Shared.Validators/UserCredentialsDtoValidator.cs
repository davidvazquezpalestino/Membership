using Membership.Shared.ValueObjects;

namespace Membership.Shared.Validators;
internal class UserCredentialsDtoValidator : AbstractValidator<UserCredentialsDto>
{
    public UserCredentialsDtoValidator(IMembershipMessageLocalizer localizer) :
        base(localizer)
    {
    }

    protected override void ValidatePropertyRules(UserCredentialsDto entity,
        string propertyName, List<MembershipError> errors)
    {
        switch(propertyName)
        {
            case nameof(UserCredentialsDto.Email):
                ValidateRule(() => !string.IsNullOrWhiteSpace(entity.Email),
                    propertyName, MessageKeys.RequiredEmailErrorMessage,
                    errors);
                break;
            case nameof(UserCredentialsDto.Password):
                ValidateRule(() => !string.IsNullOrWhiteSpace(entity.Password),
                    propertyName, MessageKeys.RequiredPasswordErrorMessage,
                    errors);
                break;
        }
    }
}
