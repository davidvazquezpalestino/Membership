using Membership.Shared.ValueObjects;

namespace Membership.Shared.Validators;
internal class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator(IMembershipMessageLocalizer localizer) :
        base(localizer)
    {
    }

    protected override void ValidatePropertyRules(UserDto entity,
        string propertyName, List<MembershipError> errors)
    {
        switch (propertyName)
        {
            case nameof(UserDto.FirstName):
                ValidateRule(() => !string.IsNullOrWhiteSpace(entity.FirstName),
                    propertyName, MessageKeys.RequiredFirstNameErrorMessage,
                    errors);
                break;
            case nameof(UserDto.LastName):
                ValidateRule(() => !string.IsNullOrWhiteSpace(entity.LastName),
                    propertyName, MessageKeys.RequiredLastNameErrorMessage,
                    errors);
                break;
            case nameof(UserDto.Email):
                ValidateRule(() => !string.IsNullOrWhiteSpace(entity.Email),
                    propertyName, MessageKeys.RequiredEmailErrorMessage,
                    errors);
                break;
            case nameof(UserDto.Password):
                if (ValidateRule(() => !string.IsNullOrWhiteSpace(entity.Password),
                    propertyName, MessageKeys.RequiredPasswordErrorMessage,
                    errors))
                {
                    ValidateRule(() => entity.Password.Length >= 6,
                    propertyName, MessageKeys.PasswordTooShortErrorMessage,
                    errors);
                    ValidateRule(() => entity.Password.Any(c => char.IsLower(c)),
                        propertyName, MessageKeys.PasswordRequiresLowerErrorMessage,
                        errors);
                    ValidateRule(() => entity.Password.Any(c => char.IsUpper(c)),
                        propertyName, MessageKeys.PasswordRequiresUpperErrorMessage,
                        errors);
                    ValidateRule(() => entity.Password.Any(c => char.IsDigit(c)),
                        propertyName, MessageKeys.PasswordRequiresDigitErrorMessage,
                        errors);
                    ValidateRule(() => entity.Password.Any(
                        c => !char.IsLetterOrDigit(c)),
                        propertyName,
                        MessageKeys.PasswordRequiresNonAlphanumericErrorMessage,
                        errors);
                }
                break;
            case nameof(UserDto.ConfirmPassword):
                    ValidateRule(() => entity.Password == entity.ConfirmPassword,
                        propertyName, MessageKeys.CompareConfirmPasswordErrorMessage,
                        errors);
                break;
        }
    }
}
