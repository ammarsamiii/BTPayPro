namespace BTPayPro.Api.Models
{
    public class UserRegistrationDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } = string.Empty;
        public int? UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

}
