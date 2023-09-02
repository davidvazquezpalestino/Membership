using Membership.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Pages;

public partial class Login
{
    const string RouteTemplate = "/user/login";

    [Inject]
    NavigationManager NavigationManager { get; set; }

    void OnLogin(UserTokensDto tokens)
    {
        NavigationManager.NavigateTo(GetReturnUri());
    }


    string GetReturnUri()
    {
        string NavigateTo = NavigationManager.Uri.EndsWith(RouteTemplate) ? ""
            : NavigationManager.Uri;
        return NavigateTo;
    }
}