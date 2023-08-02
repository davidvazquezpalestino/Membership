namespace Membership.Abstractions.Interfaces.Login;
public interface ILoginOutputPort
{
    UserTokensDto UserTokens { get; }
    Task HandleUserEntityAsync(UserEntity user);
}
