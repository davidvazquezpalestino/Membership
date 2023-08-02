namespace Membership.RefreshTokenService.MemoryCache;
public static class DependencyContainer
{
    public static IServiceCollection AddRefreshTokenMemoryCacheService(
        this IServiceCollection services)
    {
        services.TryAddSingleton<IMemoryCache,
            Microsoft.Extensions.Caching.Memory.MemoryCache>();
        services.TryAddSingleton<IRefreshTokenService, RefreshTokenService>();

        return services;
    }
}

