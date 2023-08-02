using Membership.Shared.ValueObjects;

namespace Membership.Shared.Abstracts;
public abstract class AbstractValidator<T> : IValidator<T>
{
    readonly IMembershipMessageLocalizer Localizer;
    public AbstractValidator(IMembershipMessageLocalizer localizer) =>
        Localizer = localizer;

    public IEnumerable<MembershipError> Validate(T entity)
    {
        List<MembershipError> Errors = new();
        var Properties = typeof(T).GetProperties();
        foreach (var Property in Properties)
        {
            var PropertyErrors = ValidateProperty(entity, Property.Name);
            if (PropertyErrors.Any()) Errors.AddRange(PropertyErrors);
        }
        return Errors;
    }

    protected abstract void ValidatePropertyRules(T entity, string propertyName,
        List<MembershipError> errors);

    public IEnumerable<MembershipError> ValidateProperty(T entity, string propertyName)
    {
        List<MembershipError> Errors = new();
        ValidatePropertyRules(entity, propertyName, Errors);
        return Errors;
    }

    protected bool ValidateRule(Func<bool> predicate, string propertyName,
        string errorMessageKey, List<MembershipError> errors)
    {
        bool Result = true;
        if (!predicate())
        {
            errors.Add(new(propertyName, Localizer[errorMessageKey]));
            Result = false;
        }
        return Result;
    }
}
