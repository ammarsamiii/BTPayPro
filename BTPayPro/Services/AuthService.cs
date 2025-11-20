// AuthService.cs
using BTPayPro.Domaine;
using BTPayPro.Services;
using System.Security.Claims;
using BTPayPro.Interfaces;

using BTPayPro.Security;
using BTPayPro.Services;

namespace BTPayPro.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepositories<User> _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IRepositories<User> userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<User> Register(User user, string password)
        {
            user.PasswordHash = PasswordHasher.HashPassword(password);
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = (await _userRepository.FindAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // Refresh token valid for 7 days
            _userRepository.Update(user);

            return accessToken; // In a real app, you'd return both tokens
        }

        public async Task<string> RefreshToken(string accessToken, string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            return newAccessToken;
        }
    }
}