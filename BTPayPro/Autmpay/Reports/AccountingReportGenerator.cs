using BTPayPro.Autmpay.Models;
using System.Text;

namespace BTPayPro.Autmpay.Reports
{
    public class AccountingReportGenerator
    {
        public string GenerateMarkdownReport(List<DetailRecord> detailRecords)
        {
            if (detailRecords == null || detailRecords.Count == 0)
            {
                return "# Rapport Comptable AUTMPAY\n\nAucun enregistrement de détail trouvé.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Rapport Comptable AUTMPAY\n");
            sb.AppendLine("Ce rapport présente les détails des transactions AUTMPAY.\n");

            sb.AppendLine("| Numéro de Séquence | Code Opération | Date Traitement | Montant Autorisé | Montant Règle | Nom Marchand | Ville Marchand | Statut Opération |");
            sb.AppendLine("|--------------------|----------------|-----------------|------------------|---------------|--------------|----------------|------------------|");

            foreach (var record in detailRecords)
            {
                sb.AppendLine($"| {record.RecordSequenceNumber} | {record.OperationCode} | {record.ProcessingDate} | {FormatAmount(record.TransactionAuthorizedAmount)} | {FormatAmount(record.TransactionSettlementAmount)} | {record.MerchantName} | {record.MerchantCity} | {record.OperationStatus} |");
            }

            return sb.ToString();
        }

        private string FormatAmount(string amountString)
        {
            if (string.IsNullOrEmpty(amountString)) return "0.00";
            // Assuming the amount is a fixed-point number with 3 decimal places (V9(3) from spec)
            if (amountString.Length <= 3) return "0." + amountString.PadLeft(3, '0');
            return amountString.Insert(amountString.Length - 3, ".");
        }
    }
}
