namespace Membership.Core.Controllers;
internal class TokenController
{
    public static void Map(WebApplication app)
    {
        app.MapPost(MembershipEndpoints.Token,
            async (HttpContext context, ITokenInputPort inputPort,
            IOAuthService oAuthService, ILoginOutputPort presenter) =>
            {
                var RequestBody = context.Request.Form.ToDictionary(i => i.Key,
                    i => i.Value.ToString());
                var RequestInfo = oAuthService
                .GetTokenRequestInfoFromRequestBody(RequestBody);

                await inputPort.HandleTokenRequestAsync(RequestInfo);
                context.Response.Headers.Add("Cache-Control", "no-store");
                return Results.Ok(presenter.UserTokens);
            });
    }
}
