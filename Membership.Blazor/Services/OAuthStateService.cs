namespace Membership.Blazor.Services;
internal class OAuthStateService : IOAuthStateService
{
    readonly IJSRuntime JsRuntime;
    private readonly ILogger<OAuthStateService> Logger;

    public OAuthStateService(IJSRuntime jSRuntime, ILogger<OAuthStateService> logger)
    {
        JsRuntime = jSRuntime;
        Logger = logger;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            string serializedValue = JsonSerializer.Serialize(value);
            string valueToSave =
                Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedValue));
            await JsRuntime.InvokeVoidAsync("sessionStorage.setItem",
                key, valueToSave);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
        }
    }

    public async Task<T> GetAsync<T>(string key)
    {
        T value = default;
        try
        {
            string savedValue = await JsRuntime.InvokeAsync<string>(
                "sessionStorage.getItem", key);
            if (savedValue != null)
            {
                string serializedValue =
                    Encoding.UTF8.GetString(Convert.FromBase64String(savedValue));
                value = JsonSerializer.Deserialize<T>(serializedValue);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
        }
        return value;
    }

    public async Task RemoveAsync(string key) =>
        await JsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
}
