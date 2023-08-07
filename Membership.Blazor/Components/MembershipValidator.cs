using Membership.Shared.ValueObjects;

namespace Membership.Blazor.Components;
public class MembershipValidator<T> : ComponentBase
{
    [CascadingParameter]
    EditContext EditContext { get; set; }

    [Parameter]
    public IValidator<T> Validator { get; set; }

    ValidationMessageStore ValidationMessageStore;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        EditContext previousEditContext = EditContext;
        await base.SetParametersAsync(parameters);
        if (EditContext != previousEditContext)
        {
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            EditContext.OnValidationRequested += ValidationRequested;
            EditContext.OnFieldChanged += FieldChanged;
        }
    }

    void HandleErrors(object model, IEnumerable<MembershipError> errors)
    {
        List<MembershipError> membershipErrors = errors.ToList();
        if (membershipErrors.Any())
        {
            foreach (MembershipError error in membershipErrors)
            {
                ValidationMessageStore.Add(new FieldIdentifier(model, error.Code), error.Description);
            }
        }
    }

    void ValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
        ValidationMessageStore.Clear();
        IEnumerable<MembershipError> result = Validator.Validate((T)EditContext.Model);
        HandleErrors(EditContext.Model, result);
    }

    void FieldChanged(object sender, FieldChangedEventArgs e)
    {
        FieldIdentifier fieldIdentifier = e.FieldIdentifier;
        ValidationMessageStore.Clear(fieldIdentifier);
        IEnumerable<MembershipError> result = Validator.ValidateProperty((T)fieldIdentifier.Model, fieldIdentifier.FieldName);
        HandleErrors(fieldIdentifier.Model, result);
    }

    public void TrySetErrorsFromHttpRequestException(HttpRequestException ex)
    {
        if (ex.Data.Contains("Errors"))
        {
            IEnumerable<MembershipError> errors = (List<MembershipError>)ex.Data["Errors"];
            
            if (errors != null && errors.Any())
            {
                ValidationMessageStore.Clear();
                HandleErrors(EditContext.Model, errors);
                EditContext.NotifyValidationStateChanged();
            }
        }
    }
}
