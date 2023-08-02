namespace Membership.Abstractions.Interfaces.Token;
public interface ITokenInputPort
{
    Task HandleTokenRequestAsync(TokenRequestInfo info);
}
