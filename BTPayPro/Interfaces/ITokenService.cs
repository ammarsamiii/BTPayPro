
using BTPayPro.Domaine;
using System.Security.Claims;

namespace BTPayPro.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}