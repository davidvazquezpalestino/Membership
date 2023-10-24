namespace Membership.Core.Controllers;
internal class AuthorizeController
{
    public static void Map(WebApplication app)
    {
        app.MapGet(MembershipEndpoints.Authorize,
            async (HttpRequest request, IAuthorizeInputPort inputPort) =>
            {
                AppClientAuthorizeRequestInfo requestInfo = new AppClientAuthorizeRequestInfo()
                {
                    ClientId = request.Query["client_id"],
                    RedirectUri = request.Query["redirect_uri"],
                    Scope = request.Query["scope"],
                    State = request.Query["state"],
                    CodeChallenge = request.Query["code_challenge"],
                    CodeChallengeMethod = request.Query["code_challenge_method"],
                    Nonce = request.Query["nonce"]
                };
                string redirectUri =
                await inputPort.GetAuthorizeRequestRedirectUri(requestInfo);
                return Results.Redirect(redirectUri);
            }
            );
    }
}
