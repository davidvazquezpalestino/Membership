namespace Membership.Core.Interactors;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipInteractors(
        this IServiceCollection services)
    {
        services.AddScoped<IRegisterInputPort, RegisterInteractor>();
        services.AddScoped<ILoginInputPort, LoginInteractor>();
        services.AddScoped<ILogoutInputPort, LogoutInteractor>();
        services.AddScoped<IRefreshTokenInputPort, RefreshTokenInteractor>();
        services.AddScoped<IAuthorizeInputPort, AuthorizeInteractor>();
        services.AddScoped<IAuthorizeCallbackInputPort, AuthorizeCallbackInteractor>();
        services.AddScoped<ITokenInputPort, TokenInteractor>();
        return services;
    }
}

