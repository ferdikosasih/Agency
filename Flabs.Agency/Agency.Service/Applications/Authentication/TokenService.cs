using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Agency.Service.Authentication;

public interface ITokenService
{
    string GenerateToken(string userId, string userName, string roleName);
}

public class TokenService : ITokenService
{
    public string GenerateToken(string userId, string userName, string roleName)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText("private-key.pem").ToCharArray());

        var signingKey = new RsaSecurityKey(rsa);
        var credentials =  new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);
        
        var now  = DateTime.UtcNow;

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Role, roleName)
            }),
            Issuer = "Agency.Service.Authentication",
            Audience = "Agency.Service",
            IssuedAt = now,
            Expires = now.AddHours(1),
            SigningCredentials = credentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}