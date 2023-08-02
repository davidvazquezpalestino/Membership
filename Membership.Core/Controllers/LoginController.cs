namespace Membership.Core.Controllers;
internal class LoginController 
{
    public static void Map(WebApplication app)
    {
        app.MapPost(MembershipEndpoints.Login,
            async (HttpContext context, UserCredentialsDto userCredentials, 
            ILoginInputPort inputPort, ILoginOutputPort outputPort) =>
            {
                await inputPort.LogingAsync(userCredentials);
                context.Response.Headers.Add("Cache-Control", "no-store");
                return Results.Ok(outputPort.UserTokens);
            });
    }
}
