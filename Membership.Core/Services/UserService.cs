namespace Membership.Core.Services;
internal class UserService : IUserService
{
    readonly IHttpContextAccessor Context;

    public UserService(IHttpContextAccessor context)
    {
        Context = context;
    }

    public bool IsAuthenticated =>
        Context.HttpContext.User.Identity.IsAuthenticated;

    public string Email =>
        Context.HttpContext.User.Identity.Name;

    public string FullName =>
        Context.HttpContext.User.Claims
        .Where(claim => claim.Type == "FullName")
        .Select(claim => claim.Value).FirstOrDefault();
}
