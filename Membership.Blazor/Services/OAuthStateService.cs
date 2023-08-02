namespace Membership.Blazor.Services;
internal class OAuthStateService : IOAuthStateService
{
    readonly IJSRuntime JSRuntime;

    public OAuthStateService(IJSRuntime jSRuntime)
    {
        JSRuntime = jSRuntime;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            var SerializedValue = JsonSerializer.Serialize(value);
            var ValueToSave =
                Convert.ToBase64String(Encoding.UTF8.GetBytes(SerializedValue));
            await JSRuntime.InvokeVoidAsync("sessionStorage.setItem",
                key, ValueToSave);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<T> GetAsync<T>(string key)
    {
        T Value = default;
        try
        {
            var SavedValue = await JSRuntime.InvokeAsync<string>(
                "sessionStorage.getItem", key);
            if (SavedValue != null)
            {
                var SerializedValue =
                    Encoding.UTF8.GetString(Convert.FromBase64String(SavedValue));
                Value = JsonSerializer.Deserialize<T>(SerializedValue);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return Value;
    }

    public async Task RemoveAsync(string key) =>
        await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
}
