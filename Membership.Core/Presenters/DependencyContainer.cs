namespace Membership.Core.Presenters;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipPresenters(
        this IServiceCollection services)
    {
        services.AddScoped<ILoginOutputPort, LoginPresenter>();
        services.AddScoped<IRefreshTokenOutputPort, RefreshTokenPresenter>();

        return services;
    }
}

