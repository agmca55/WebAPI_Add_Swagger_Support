namespace WebAPI.Services
{
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;


    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims, out string jwtId);
        string GenerateRefreshToken();
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config) => _config = config;

        public string GenerateAccessToken(IEnumerable<Claim> claims, out string jwtId)
        {
            jwtId = Guid.NewGuid().ToString();
            var allClaims = claims.ToList();
            allClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, jwtId)); // Explicitly bind Jti

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: allClaims,
                expires: DateTime.UtcNow.AddMinutes(15), // Short access token lifetime
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }

}
