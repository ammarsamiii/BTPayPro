using BTPayPro.Api.Models;
using BTPayPro.data;
using BTPayPro.Domaine;
using BTPayPro.Enums;
using BTPayPro.Security;
using BTPayPro.Services;
using Microsoft.AspNetCore.Mvc;

namespace BTPayPro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenService _jwtService;

        public AuthController(AppDbContext dbContext, ITokenService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User dto)
        {
            bool emailExists = _dbContext.Users.Any(u => u.Email == dto.Email);

            //if (_dbContext.Users.Any(u => u.Email == dto.Email))
            //  return BadRequest("Email already exists");
            if (emailExists)
            {
                return BadRequest("Email already exists");
            }
            // Hash the password before saving
            dto.PasswordHash = PasswordHasher.HashPassword(dto.PasswordHash);
            _dbContext.Users.Add(dto);
            _dbContext.SaveChanges();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto dto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            if (!PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash)) // static call
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateAccessToken(user);

            Console.WriteLine($"User type: {user.UserType} (int value: {(int)user.UserType})");
            string dashboardPath = user.UserType switch
            {
                UserType.Client => "/dashboards/dashboard-client.html",
                UserType.Merchant => "/dashboards/dashboard-vendor.html",
                UserType.Admin => "/dashboards/dashboard-admin.html",
                _ => "/login.html"  // Default to login if user type is unknown
            };
            Console.WriteLine($"Selected dashboard path: {dashboardPath}");

            Console.WriteLine($"Login: User {user.Email} has UserType={user.UserType}, RedirectUrl={dashboardPath}");

            // Get wallet balance if user is Client or Merchant
            double? walletBalance = null;
            if (user.UserType == UserType.Client || user.UserType == UserType.Merchant)
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserId == user.IdUser);
                walletBalance = wallet?.Balance;
            }

            // Return user info with token
            return Ok(new
            {
                token = token,
                redirectUrl = dashboardPath,
                userType = user.UserType.ToString(),
                userName = string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName)
                    ? user.Email
                    : $"{user.FirstName} {user.LastName}".Trim(),
                email = user.Email,
                userId = user.IdUser,
                walletBalance = walletBalance
            });
        }
    }
}
