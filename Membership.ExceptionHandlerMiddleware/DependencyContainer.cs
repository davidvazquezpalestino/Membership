namespace Membership.ExceptionHandlerMiddleware;
public static class DependencyContainer
{
    public static IApplicationBuilder UseMembershipExceptionHandler(
        this IApplicationBuilder app)
    {
        AddHandlers();

        app.UseExceptionHandler(builder =>
        {
            builder.Run(async (context) =>
            await MembershipExceptionHandler.WriteResponse(context,
                app.ApplicationServices.GetRequiredService<IMembershipMessageLocalizer>()));
        });

        return app;
    }

    static void AddHandlers()
    {
        MembershipExceptionHandler.AddHandler(typeof(RegisterUserException),
            (RegisterUserException ex,
            IMembershipMessageLocalizer localizer) =>
            new ProblemDetails().FromHttp400BadRequest(
                localizer[MessageKeys.RegisterUserExceptionMessage], nameof(RegisterUserException), ex.Errors));

        MembershipExceptionHandler.AddHttp400Handler<LoginUserException>();
        MembershipExceptionHandler.AddHttp400Handler<RefreshTokenCompromisedException>();
        MembershipExceptionHandler.AddHttp400Handler<RefreshTokenExpiredException>();
        MembershipExceptionHandler.AddHttp400Handler<RefreshTokenNotFoundException>();
        MembershipExceptionHandler.AddHttp400Handler<MissingCallbackStateParameterException>();
        MembershipExceptionHandler.AddHttp400Handler<UnableToGetIDPTokensException>();
        MembershipExceptionHandler.AddHttp400Handler<InvalidAuthorizationCodeException>();
        MembershipExceptionHandler.AddHttp400Handler<InvalidRedirectUriException>();
        MembershipExceptionHandler.AddHttp400Handler<InvalidClientIdException>();
        MembershipExceptionHandler.AddHttp400Handler<InvalidScopeException>();
        MembershipExceptionHandler.AddHttp400Handler<InvalidCodeVerifierException>();
        MembershipExceptionHandler.AddHttp400Handler<InvalidScopeActionException>();

        MembershipExceptionHandler.AddHandler(typeof(UnauthorizedAccessException),
            (UnauthorizedAccessException ex, IMembershipMessageLocalizer localizer) =>
            new ProblemDetails()
            {
                Title = nameof(UnauthorizedAccessException),
                Status = 401
            });
    }
}


