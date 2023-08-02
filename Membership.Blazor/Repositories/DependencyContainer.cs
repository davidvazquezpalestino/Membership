namespace Membership.Blazor.Repositories;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipRepository(
        this IServiceCollection services)
    {

        services.AddScoped<ITokensRepository, TokensRepository>();
        return services;
    }
}

