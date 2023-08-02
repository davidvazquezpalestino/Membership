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
        EditContext PreviousEditContext = EditContext;
        await base.SetParametersAsync(parameters);
        if (EditContext != PreviousEditContext)
        {
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            EditContext.OnValidationRequested += ValidationRequested;
            EditContext.OnFieldChanged += FieldChanged;
        }
    }

    void HandleErrors(object model, IEnumerable<MembershipError> errors)
    {
        if (errors != null && errors.Any())
        {
            foreach (var Error in errors)
            {
                ValidationMessageStore.Add(
                    new FieldIdentifier(model, Error.Code),
                    Error.Description);
            }
        }
    }

    void ValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
        ValidationMessageStore.Clear();
        var Result = Validator.Validate((T)EditContext.Model);
        HandleErrors(EditContext.Model, Result);
    }

    void FieldChanged(object sender, FieldChangedEventArgs e)
    {
        FieldIdentifier FieldIdentifier = e.FieldIdentifier;
        ValidationMessageStore.Clear(FieldIdentifier);
        var Result =
            Validator.ValidateProperty((T)FieldIdentifier.Model,
            FieldIdentifier.FieldName);
        HandleErrors(FieldIdentifier.Model, Result);
    }

    public void TrySetErrorsFromHttpRequestException(HttpRequestException ex)
    {
        if (ex.Data.Contains("Errors"))
        {
            IEnumerable<MembershipError> Errors = ex.Data["Errors"] as
                IEnumerable<MembershipError>;
            if (Errors != null && Errors.Any())
            {
                ValidationMessageStore.Clear();
                HandleErrors(EditContext.Model, Errors);
                EditContext.NotifyValidationStateChanged();
            }
        }
    }

}
