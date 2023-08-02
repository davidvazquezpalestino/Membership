namespace Membership.Blazor.Gateways;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipGateways(
        this IServiceCollection services,
        Action<UserEndpointsOptions> userEndpointsOptionsSetter)
    {
        services.AddOptions<UserEndpointsOptions>()
            .Configure(userEndpointsOptionsSetter);

        services.AddHttpClient<IUserWebApiGateway, UserWebApiGateway>()
            .AddExceptionDelegatingHandler();

        return services;
    }
}

