namespace Membership.Blazor.Interfaces;
internal interface ITokensRepository
{
    Task SaveTokensAsync(UserTokensDto userTokens);
    Task<UserTokensDto> GetTokensAsync();
    Task RemoveTokensAsync();

}
