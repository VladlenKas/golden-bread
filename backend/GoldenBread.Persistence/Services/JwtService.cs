using GoldenBread.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Services;

public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateToken(string accountId, string email, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var tokenClaims = claims.Append(new Claim(ClaimTypes.NameIdentifier, accountId))
                                .Append(new Claim(ClaimTypes.Email, email));

        var tokenDescryptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHander = new JwtSecurityTokenHandler();
        return tokenHander.CreateEncodedJwt(tokenDescryptor);
    }

    public string? GenerateRefreshToken() => 
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
