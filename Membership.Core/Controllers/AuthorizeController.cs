using Membership.Abstractions.Interfaces.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.Core.Controllers;
internal class AuthorizeController
{
    public static void Map(WebApplication app)
    {
        app.MapGet(MembershipEndpoints.Authorize,
            async(HttpRequest request, IAuthorizeInputPort inputPort) =>
            {
                var RequestInfo = new AppClientAuthorizeRequestInfo()
                {
                    ClientId = request.Query["client_id"],
                    RedirectUri = request.Query["redirect_uri"],
                    Scope = request.Query["scope"],
                    State = request.Query["state"],
                    CodeChallenge = request.Query["code_challenge"],
                    CodeChallengeMethod = request.Query["code_challenge_method"],
                    Nonce = request.Query["nonce"]
                };
                string RedirectUri =
                await inputPort.GetAuthorizeRequestRedirectUri(RequestInfo);
                return Results.Redirect(RedirectUri);
            }
            );
    }
}
