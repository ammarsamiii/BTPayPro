using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPayPro.Domaine
{
    [Table("Accountings")]
    public class Accounting
    {


        [Key]
        public string AccountId { get; set; }
        // PDF file stored as byte[] - nullable
        public byte[]? Monetique { get; set; }

        // Optional: MIME type to ensure it's a PDF - nullable
        [MaxLength(50)]
        public string? MonetiqueContentType { get; set; }

        // Navigation property for Wallet (1-to-1 relationship)
        public Wallet? Wallet { get; set; }

        // Methods (not directly mapped to database columns, but included for completeness)
        public void RecordTransaction(Transaction transaction) { /* Implementation */ }
        public double GetBalance() { return 0.0; /* Implementation */ }
    }
}
