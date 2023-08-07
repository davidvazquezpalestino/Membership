namespace Membership.Core.Controllers;
internal class AuthorizeCallbackController
{
    public static void Map(WebApplication app)
    {
        app.MapGet(MembershipEndpoints.AuthorizeCallback,
            async (IAuthorizeCallbackInputPort inputPort, [FromQuery] string state, [FromQuery] string code) =>
            Results.Redirect(await inputPort.HandleCallback(state, code))
            );
    }
}
