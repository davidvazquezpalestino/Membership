namespace Membership.Blazor.Repositories;
internal class TokensRepository : ITokensRepository
{
    const string SessionKey = "asp";
    readonly IJSRuntime JSRuntime;
    public TokensRepository(IJSRuntime jSRuntime)
    {
        JSRuntime = jSRuntime;
    }

    public async Task<UserTokensDto> GetTokensAsync()
    {
        UserTokensDto StoredTokens = default;
        string Value = await JSRuntime.InvokeAsync<string>(
            "sessionStorage.getItem", SessionKey);
        if (Value != null)
        {
            string SerializedTokens =
                Encoding.UTF8.GetString(Convert.FromBase64String(Value));
            StoredTokens = JsonSerializer
                .Deserialize<UserTokensDto>(SerializedTokens);
        }
        return StoredTokens;
    }

    public async Task RemoveTokensAsync() =>
        await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);


    public async Task SaveTokensAsync(UserTokensDto userTokens)
    {
        string SerializedTokens = JsonSerializer.Serialize(userTokens);
        string Value = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(SerializedTokens));
        await JSRuntime.InvokeVoidAsync("sessionStorage.setItem",
            SessionKey, Value);
    }
}
