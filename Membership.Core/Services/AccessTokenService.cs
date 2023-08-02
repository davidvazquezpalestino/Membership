namespace Membership.Core.Services;
internal class AccessTokenService : IAccessTokenService
{
    readonly JwtOptions JwtOptions;
    public AccessTokenService(IOptions<JwtOptions> jwtOptions) =>
        JwtOptions = jwtOptions.Value;

    public Task<string> GetNewUserAccessTokenAsync(UserEntity user) =>
        Task.FromResult(GetAccessToken(GetUserClaims(user)));

    public Task<string> RotateAccessTokenAsync(string accessTokenToRotate) =>
        Task.FromResult(GetAccessToken(
            GetUserClaimsFromAccessToken(accessTokenToRotate)));

    string GetAccessToken(List<Claim> userClaims)
    {
        var Key = Encoding.UTF8.GetBytes(JwtOptions.SecurityKey);
        var Secret = new SymmetricSecurityKey(Key);
        var SigningCredentials = new SigningCredentials(Secret,
            SecurityAlgorithms.HmacSha256);

        var TokenOptions = new JwtSecurityToken(
            issuer: JwtOptions.ValidIssuer,
            audience: JwtOptions.ValidAudience,
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(JwtOptions.ExpireInMinutes),
            signingCredentials: SigningCredentials);
        return new JwtSecurityTokenHandler().WriteToken(TokenOptions);
    }

    static List<Claim> GetUserClaims(UserEntity user)
    {
        var DefaultUserClaims = 
            new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", $"{user.FullName}")
            };
        if (user.Claims != null) DefaultUserClaims.AddRange(user.Claims);
        return DefaultUserClaims;
    }
    static List<Claim> GetUserClaimsFromAccessToken(string accesToken) =>
        new JwtSecurityTokenHandler()
        .ReadJwtToken(accesToken)
        .Claims
        .ToList();
}
