using Membership.Shared.OAuth;

namespace Membership.Blazor;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipBlazorServices(
        this IServiceCollection services,
        Action<UserEndpointsOptions> userEndpointsOptionsSetter,
        Action<AppOptions> appOptionsSetter)
    {
        services.AddAuthorizationCore();
        services.AddMembershipAuthenticationStateProvider();
        services.AddMembershipGateways(userEndpointsOptionsSetter);
        services.AddMembershipRepository();
        services.AddMembershipServices(appOptionsSetter);
        services.AddOAuthService(); 

        return services;
    }
}
