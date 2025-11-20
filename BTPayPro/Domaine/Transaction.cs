using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPayPro.Domaine
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public double TransactionAmount { get; set;}
        public DateOnly TransactionDate { get; set; }
        public string Status { get; set; }
        public double  Comission { get; set; }
        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
        // New field for external gateway ID
        public string? ExternalOrderId { get; set; }

    }
}
