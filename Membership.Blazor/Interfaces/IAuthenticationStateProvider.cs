namespace Membership.Blazor.Interfaces;
internal interface IAuthenticationStateProvider
{
    Task<AuthenticationState> GetAuthenticationStateAsync();
    Task LoginAsync(UserTokensDto userTokensDto);
    Task LogoutAsync();
    Task<UserTokensDto> GetUserTokensAsync();
}
