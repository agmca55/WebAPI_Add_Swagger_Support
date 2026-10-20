using WebAPI.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace WebAPI.Services 
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        public AuthenticateService (IOptions<AppSettings> appSetting)
        {
            _appSettings = appSetting.Value;
        }
        private List<User> users = new List<User>()
        {
            new User{UserId=1,UserName="admin",Password="admin",FirstName="Admin Anil",LastName="Gautam",Role="Admin"},
            new User{UserId=2,UserName="user", Password="user",FirstName="User Ravi",LastName="Kapoor",Role="User"}
        };
        public User Authenticate(string username, string password)
        {
            User user = users.Where(m => m.UserName == username && m.Password == password).SingleOrDefault();

            if(user==null)
            {
                return null;
            }

            //is user found
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Version, "v.001")
                }),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = null;

            return user;
        }
    }
}
