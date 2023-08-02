namespace Membership.Core;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipCoreServices(
        this IServiceCollection services,
        Action<JwtOptions> jwtOptionsSetter,
        Action<AppClientInfoOptions> appClientInfoOptionsSetter,
        Action<IDPClientInfoOptions> iDPClientInfoOptionsSetter)
    {
        services.AddMembershipInteractors()
            .AddMembershipPresenters()
            .AddMembershipInternalServices(jwtOptionsSetter,
            appClientInfoOptionsSetter, iDPClientInfoOptionsSetter)
            .AddOAuthService();

        return services;
    }

    public static WebApplication UseMembershipEndpoints(this WebApplication app) =>
        app.UseMembershipControllers();
}

