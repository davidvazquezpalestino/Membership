namespace Membership.RefreshTokenService.MemoryCache;
internal class RefreshTokenService : IRefreshTokenService
{
    readonly JwtOptions JwtOptions;
    readonly IMemoryCache Cache;

    public RefreshTokenService(IOptions<JwtOptions> jwtOptions, IMemoryCache cache)
    {
        JwtOptions = jwtOptions.Value;
        Cache = cache;
    }

    public Task DeleteRefreshTokenAsync(string refreshToken)
    {
        Cache.Remove(refreshToken);
        return Task.CompletedTask;
    }

    public Task<string> GetRefreshTokenForAccessTokenAsync(string accessToken)
    {
        var RefreshToken = GenerateToken();
        RefreshTokenInfo RefreshTokenInfo = new(accessToken,
            DateTime.UtcNow.AddMinutes(JwtOptions.RefreshTokenExpireInMinutes));
        Cache.Set(RefreshToken, RefreshTokenInfo,
            DateTime.Now.AddMinutes(
                JwtOptions.RefreshTokenExpireInMinutes + 5));
        return Task.FromResult(RefreshToken);
    }

    public Task ThrowIfUnableToRotateRefreshTokenAsync(string refreshToken,
        string accessToken)
    {
        if (Cache.TryGetValue(refreshToken,
            out RefreshTokenInfo refreshTokenInfo))
        {
            Cache.Remove(refreshToken);
            if (refreshTokenInfo.AccessToken != accessToken)
                throw new RefreshTokenCompromisedException();

            if (refreshTokenInfo.RefreshTokenExpiresAt < DateTime.UtcNow)
                throw new RefreshTokenExpiredException();
        }
        else
        {
            throw new RefreshTokenNotFoundException();
        }
        return Task.CompletedTask;
    }

    static string GenerateToken()
    {
        // Cada caracter Base64 es de 6 bits
        // 3 bytes generan 4 caracteres base64
        var Buffer = new Byte[75];
        using var Rng = RandomNumberGenerator.Create();
        Rng.GetNonZeroBytes(Buffer);
        return Convert.ToBase64String(Buffer);
    }
}
