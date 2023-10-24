namespace Membership.Core.Controllers;
internal class TokenController
{
    public static void Map(WebApplication app)
    {
        app.MapPost(MembershipEndpoints.Token,
            async (HttpContext context, ITokenInputPort inputPort, IOAuthService oAuthService, ILoginOutputPort presenter) =>
            {
                Dictionary<string, string> requestBody = context.Request.Form.ToDictionary(i => i.Key, i => i.Value.ToString());

                TokenRequestInfo requestInfo = oAuthService.GetTokenRequestInfoFromRequestBody(requestBody);

                await inputPort.HandleTokenRequestAsync(requestInfo);
                context.Response.Headers.Add("Cache-Control", "no-store");

                return Results.Ok(presenter.UserTokens);

            });
    }
}
