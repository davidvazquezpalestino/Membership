namespace Membership.Shared.Validators;
public static class DependencyContainer
{
    public static IServiceCollection AddMembershipValidators(
        this IServiceCollection services)
    {
        services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
        services.AddScoped<IValidator<UserCredentialsDto>,
            UserCredentialsDtoValidator>();

        return services;
    }
}

