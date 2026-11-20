namespace Project.Security;

public interface IJwtProvider
{
    SecurityToken GenerateToken(List<Claim> authClaims,bool rememberMe);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    //void SaveRefreshToken(string userId, string refreshToken);
    //string? GetRefreshToken(string userId);
    //void RemoveRefreshToken(string userId);
    //void RemoveRefreshTokenByToken(string refreshToken);
}