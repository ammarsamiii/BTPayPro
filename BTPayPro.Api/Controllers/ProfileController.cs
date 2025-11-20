using BTPayPro.Api.Models;
using BTPayPro.data;
using BTPayPro.Domaine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTPayPro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        // GET: api/Profile/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<ProfileResponseDto>> GetProfile(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var profile = new ProfileResponseDto(
                user.IdUser,
                user.Email ?? "",
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.ProjectName,
                user.UserType?.ToString() ?? "Unknown",
                user.ProfilePhotoUrl
            );

            return Ok(profile);
        }

        // PUT: api/Profile/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile(string userId, [FromBody] ProfileUpdateDto dto)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update only the allowed fields
            if (dto.FirstName != null)
                user.FirstName = dto.FirstName;

            if (dto.LastName != null)
                user.LastName = dto.LastName;

            if (dto.PhoneNumber != null)
                user.PhoneNumber = dto.PhoneNumber;

            if (dto.ProjectName != null)
                user.ProjectName = dto.ProjectName;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating profile", error = ex.Message });
            }
        }

        // POST: api/Profile/{userId}/photo
        [HttpPost("{userId}/photo")]
        public async Task<IActionResult> UploadProfilePhoto(string userId, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file type. Only JPG, PNG, and GIF are allowed." });
            }

            // Validate file size (max 5MB)
            if (photo.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size exceeds 5MB limit." });
            }

            try
            {
                // Use a path that's accessible from the WebUI
                // Since the frontend is served from WebUI, save files there
                var webUiWwwroot = Path.Combine(Directory.GetCurrentDirectory(), "..", "BTPayPro.WebUI", "wwwroot");
                var uploadsFolder = Path.Combine(webUiWwwroot, "uploads", "profiles");

                Console.WriteLine($"WebUI wwwroot path: {webUiWwwroot}");
                Console.WriteLine($"Uploads folder: {uploadsFolder}");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Console.WriteLine($"Created uploads folder: {uploadsFolder}");
                }

                // Delete old photo if exists
                if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
                {
                    var oldPhotoPath = Path.Combine(webUiWwwroot, user.ProfilePhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                // Generate unique filename
                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Console.WriteLine($"Saving file to: {filePath}");

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                Console.WriteLine($"File saved successfully");
                Console.WriteLine($"File exists: {System.IO.File.Exists(filePath)}");

                // Update user profile photo URL (relative path from wwwroot)
                user.ProfilePhotoUrl = $"/uploads/profiles/{uniqueFileName}";
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"Returning photo URL: {user.ProfilePhotoUrl}");

                return Ok(new { message = "Photo uploaded successfully", photoUrl = user.ProfilePhotoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading photo", error = ex.Message });
            }
        }
        // GET: api/Profile/search?q={query}
        [HttpGet("search")]
        public async Task<ActionResult<List<ProfileSearchDto>>> Search([FromQuery] string q)
        {
            try
            {
                var query = (q ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(query))
                {
                    return Ok(new List<ProfileSearchDto>());
                }

                var lower = query.ToLower();

                var results = await _dbContext.Users
                    .Where(u => ((u.FirstName ?? string.Empty).ToLower().Contains(lower))
                             || ((u.LastName ?? string.Empty).ToLower().Contains(lower))
                             || ((u.Email ?? string.Empty).ToLower().Contains(lower)))
                    .Select(u => new ProfileSearchDto(
                        u.IdUser,
                        u.FirstName,
                        u.LastName,
                        u.Email ?? string.Empty,
                        u.ProfilePhotoUrl,
                        _dbContext.Wallets.Where(w => w.UserId == u.IdUser).Select(w => w.WalletId).FirstOrDefault()
                    ))
                    .Take(10)
                    .ToListAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching profiles", error = ex.Message });
            }
        }
    }
}
