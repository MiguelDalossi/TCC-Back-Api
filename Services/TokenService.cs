using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConsultorioMedico.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ConsultorioMedico.Api.Services
{
    public class TokenService
    {
        private readonly IConfiguration _cfg;
        public TokenService(IConfiguration cfg) => _cfg = cfg;

        public (string token, DateTime expiresAt) Generate(AppUser user, IList<string> roles)
        {
            var jwt = _cfg.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new(ClaimTypes.Name, user.FullName ?? user.UserName ?? user.Email ?? ""),
            };

            // role principal (se houver várias, pega a primeira para o payload simples)
            foreach (var r in roles)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var expires = DateTime.UtcNow.AddHours(8); // ajuste como preferir

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwtString, expires);
        }
    }
}
