namespace Membership.Core.Controllers;
internal class RefreshTokenController 
{
    public static void Map(WebApplication app)
    {
        app.MapPost(MembershipEndpoints.RefreshToken,
            async(HttpContext context, UserTokensDto userTokens, 
            IRefreshTokenInputPort inputPort, IRefreshTokenOutputPort outputPort) =>
            {
                await inputPort.RefreshTokenAsync(userTokens);
                context.Response.Headers.Add("Cache-Control", "no-store");
                return Results.Ok(outputPort.UserTokens);
            });
    }
}
