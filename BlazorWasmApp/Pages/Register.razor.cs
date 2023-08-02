using Membership.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Pages;

public partial class Register
{
    [Inject]
    NavigationManager NavigationManager { get; set; }
    void OnRegister(UserDto user)
    {
        Console.WriteLine($"Usuario registrado: {user.FirstName} {user.LastName}");
        NavigationManager.NavigateTo("user/login");
    }
}