using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TipsAndSteps.UserManagement.Domain.Entities;

namespace TipsAndSteps.UserManagement.Application.Abstractions;

public interface IJwtProvider
{
    string Generate(User user);
}

public sealed class JwtProvider(IConfiguration configuration) : IJwtProvider
{
    public string Generate(User user)
    {
        var secret = configuration["Jwt:Secret"] ?? "a_very_long_and_secure_secret_key_for_tips_and_steps_123!";
        var issuer = configuration["Jwt:Issuer"] ?? "tips-steps-auth";
        var audience = configuration["Jwt:Audience"] ?? "tips-steps-api";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiryHours = configuration.GetValue<int>("Jwt:ExpiryHours", 8);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(expiryHours),
            creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
