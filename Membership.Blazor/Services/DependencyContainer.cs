namespace Membership.Blazor.Services;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipServices(
        this IServiceCollection services,
        Action<AppOptions> appOptionsSetter)
    {
        services.AddScoped<IAuthorizeService, AuthorizeService>();
        services.AddScoped<IOAuthStateService, OAuthStateService>();

        services.AddOptions<AppOptions>()
            .Configure(appOptionsSetter);

        services.AddHttpClient<TokenService>()
            .AddExceptionDelegatingHandler();

        return services;
    }
}

