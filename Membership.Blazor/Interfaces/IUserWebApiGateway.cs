namespace Membership.Blazor.Interfaces;
internal interface IUserWebApiGateway
{
    Task RegisterUserAsync(UserDto user);
    Task<UserTokensDto> LoginAsync(UserCredentialsDto userCredentials);
    Task<UserTokensDto> RefreshTokenAsync(UserTokensDto userTokens);
    Task LogoutAsync(UserTokensDto userTokens);
}
