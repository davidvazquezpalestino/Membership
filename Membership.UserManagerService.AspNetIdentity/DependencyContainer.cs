namespace Membership.UserManagerService.AspNetIdentity;
public static class DependencyContainer
{
    public static IServiceCollection AddUserManagerAspNetIdentityService(
        this IServiceCollection services,
        Action<AspNetIdentityOptions> aspNetIdentityOptionsSetter)
    {
        services.AddOptions<AspNetIdentityOptions>()
            .Configure(aspNetIdentityOptionsSetter);

        services.AddDbContext<UserManagerServiceContext>();

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<UserManagerServiceContext>();

        services.AddSingleton<UserManagerErrorsHandler>();
        services.AddScoped<IUserManagerService, Services.UserManagerService>();

        return services;
    }
}

