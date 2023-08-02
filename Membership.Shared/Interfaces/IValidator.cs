using Membership.Shared.ValueObjects;

namespace Membership.Shared.Interfaces;
public interface IValidator<T>
{
    IEnumerable<MembershipError> Validate(T entity);
    IEnumerable<MembershipError> ValidateProperty(T entity, string propertyName);
}
