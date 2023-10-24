namespace Membership.Shared.OAuth;
public static class DependencyContainer
{
    public static IServiceCollection AddOAuthService(this IServiceCollection services)
    {
        services.TryAddSingleton<IOAuthService, OAuthService>();
        return services;
    }
}

