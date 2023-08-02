namespace Membership.Core.Controllers;
internal class RegisterController 
{
    public static void Map(WebApplication app)
    {
        app.MapPost(MembershipEndpoints.Register,
            async (UserDto user, IRegisterInputPort inputPort) =>
            {
                await inputPort.RegisterAsync(user);
                return Results.Ok();
            });
    }
}
