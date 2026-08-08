using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryService.Domain;
using LibraryService.Domain.DTO;
using Microsoft.IdentityModel.Tokens;

namespace LibraryService.Infrastructure.Security;

public class TokenGenerator
{
    public string GenerateToken(User user, JwtSettings jwtSettings)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: cred
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}