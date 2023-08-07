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
        byte[] key = Encoding.UTF8.GetBytes(JwtOptions.SecurityKey);
        SymmetricSecurityKey secret = new SymmetricSecurityKey(key);
        SigningCredentials signingCredentials = new SigningCredentials(secret,
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokenOptions = new JwtSecurityToken(
            issuer: JwtOptions.ValidIssuer,
            audience: JwtOptions.ValidAudience,
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(JwtOptions.ExpireInMinutes),
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    static List<Claim> GetUserClaims(UserEntity user)
    {
        List<Claim> defaultUserClaims = 
            new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("FullName", $"{user.FullName}")
            };
        if (user.Claims != null)
        {
            defaultUserClaims.AddRange(user.Claims);
        }

        return defaultUserClaims;
    }
    static List<Claim> GetUserClaimsFromAccessToken(string accesToken) =>
        new JwtSecurityTokenHandler()
        .ReadJwtToken(accesToken)
        .Claims
        .ToList();
}
