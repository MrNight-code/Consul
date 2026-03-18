using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Consulcon.Application.Common.Settings;
using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Consulcon.Application.Services.Seguridad;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        if (user.RoleId.HasValue)
        {
            claims.Add(new Claim("roleId", user.RoleId.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim("email", user.Email));
        }

        claims.Add(new Claim("EsSuperAdmin", user.EsSuperAdmin.ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
