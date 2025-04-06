using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models;

namespace UserService.Authentication
{
    public interface IJWTToken
    {
        Task<string> GenerateJWT(UserModel user);
    }

    public class JWTToken : IJWTToken
    {
        public JWTSettings _setting;
        public JWTToken(IOptions<JWTSettings> setting)
        {
            _setting = setting.Value;
        }

        public async Task<string> GenerateJWT(UserModel user)
        {
            var key = Encoding.UTF8.GetBytes(_setting.Key);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role) // Add roles if needed
            };

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _setting.Issuer,
                audience: _setting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_setting.ExpiryMinutes)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            ));
        }
    }
}
