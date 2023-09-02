using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Membership.Core.Services;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipInternalServices(
        this IServiceCollection services,
        Action<JwtOptions> jwtOptionsSetter,
        Action<AppClientInfoOptions> appClientInfoOptionsSetter,
        Action<IDPClientInfoOptions> iDpClientInfoOptionsSetter)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IUserService, UserService>();
        services.AddOptions<JwtOptions>()
            .Configure(jwtOptionsSetter);
        services.AddSingleton<IAccessTokenService, AccessTokenService>();

        // Para IAppClientService
        services.AddOptions<AppClientInfoOptions>()
            .Configure(appClientInfoOptionsSetter);
        services.AddSingleton<IAppClientService, AppClientService>();

        // Para IIDPService
        services.AddHttpClient();
        services.AddOptions<IDPClientInfoOptions>()
            .Configure(iDpClientInfoOptionsSetter);
        services.AddSingleton<IIDPService, IdpService>();

        // Para IOAuthStateService
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        services.AddSingleton<IOAuthStateService, OAuthStateService>();

        return services;
    }
}

