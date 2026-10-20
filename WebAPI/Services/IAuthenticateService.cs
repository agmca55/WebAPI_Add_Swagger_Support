using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IAuthenticateService
    {
        User Authenticate(string username, string password);
    }
}
