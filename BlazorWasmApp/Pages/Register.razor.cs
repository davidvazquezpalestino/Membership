using Membership.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Pages;

public partial class Register
{
    [Inject]
    NavigationManager NavigationManager { get; set; }
    
    [Inject]
    ILogger<Register> Logger { get; set; }

    void OnRegister(UserDto user)
    {
        Logger.LogInformation($"Usuario registrado: {user.FirstName} {user.LastName}");
        NavigationManager.NavigateTo("user/login");
    }
}