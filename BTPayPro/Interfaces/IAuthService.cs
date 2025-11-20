
using BTPayPro.Domaine;


namespace BTPayPro.Services
{
    public interface IAuthService
    {
        Task<User> Register(User user, string password);
        Task<string> Login(string email, string password);
        Task<string> RefreshToken(string accessToken, string refreshToken);
    }
}