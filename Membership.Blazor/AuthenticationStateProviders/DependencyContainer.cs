namespace Membership.Blazor.AuthenticationStateProviders;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipAuthenticationStateProvider(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationStateProvider, 
            JwtAuthenticationStateProvider> ();

        services.AddScoped(provider => (AuthenticationStateProvider)
        provider.GetRequiredService<IAuthenticationStateProvider>());

        return services;
    }
}

