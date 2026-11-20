namespace Project.Security;

public class JwtOptions
{
    public string Secret { get; set; } = null!;
    public int TokenLifetime { get; set; }
    public int RefreshTokenLifetime {  get; set; } 
    public int TokenShortLifetime { get; set; }
    public string Issuer { get; set; }

}
