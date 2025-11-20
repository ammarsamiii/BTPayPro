using BTPayPro.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPayPro.Domaine
{
    [Table("Users")]
    public class User
    {

        [Key]
        public string IdUser { get; set; } = Guid.NewGuid().ToString();
        public UserType? UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProjectName { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public Wallet? Wallet { get; set; }
        public ICollection<Complaint>? Complaints { get; set; }

    }
}
