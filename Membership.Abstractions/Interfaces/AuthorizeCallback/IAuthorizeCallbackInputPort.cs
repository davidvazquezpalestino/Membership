namespace Membership.Abstractions.Interfaces.AuthorizeCallback;
public interface IAuthorizeCallbackInputPort
{
    Task<string> HandleCallback(string state, string code);
}
