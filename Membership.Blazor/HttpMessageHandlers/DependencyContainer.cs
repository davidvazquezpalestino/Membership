namespace Membership.Blazor.HttpMessageHandlers;
public static class DependencyContainer
{
    internal static IHttpClientBuilder AddExceptionDelegatingHandler(
        this IHttpClientBuilder builder)
    {
        builder.Services.TryAddTransient<ExceptionDelegatingHandler>();
        builder.AddHttpMessageHandler<ExceptionDelegatingHandler>();

        return builder;
    }

    public static IHttpClientBuilder AddMembershipBearerTokenHandler(
    this IHttpClientBuilder builder)
    {
        builder.Services.TryAddTransient<BearerTokenHandler>();
        builder.AddHttpMessageHandler<BearerTokenHandler>();

        return builder;
    }
}

