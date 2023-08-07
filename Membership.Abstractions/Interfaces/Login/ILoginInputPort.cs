namespace Membership.Abstractions.Interfaces.Login;
public interface ILoginInputPort
{
    Task LogingAsync(UserCredentialsDto userCredentialsDto);
}
