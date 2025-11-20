using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPayPro.Domaine
{
    [Table("Wallets")]
    public class Wallet
    {
        [Key]
        public string WalletId { get; set; } = string.Empty;

        public double Balance { get; set; }
        public double TransactionLimit { get; set; }

        // Foreign key for User (1-to-1 relationship)
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        // Navigation property for Transactions (1-to-many relationship)
        public ICollection<Transaction>? Transactions { get; set; }

        // Foreign key for Accounting (1-to-1 relationship)
        public string AccountId { get; set; } = string.Empty;
        public Accounting? Accounting { get; set; }
    }



}
