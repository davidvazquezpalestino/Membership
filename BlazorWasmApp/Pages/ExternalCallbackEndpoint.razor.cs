using Membership.Blazor.Components;
using Membership.Shared.ValueObjects;

namespace BlazorWasmApp.Pages;

public partial class ExternalCallbackEndpoint : ExternalCallbackEndpointBase
{
    string Message;
    IEnumerable<MembershipError> Errors;
    protected override void OnError(string message, IEnumerable<MembershipError> errors)
    {
        Message = message;
        Errors = errors;
    }
}