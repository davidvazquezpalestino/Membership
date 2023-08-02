namespace Membership.Core.Controllers;
public static class DependencyContainer
{
    public static WebApplication UseMembershipControllers(
        this WebApplication app)
    {
        return app.AddControllersRouteEndpoint();
    }
}

