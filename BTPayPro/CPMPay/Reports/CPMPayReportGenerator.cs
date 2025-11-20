
using BTPayPro.CPMPay.Models;
using BTPayPro.CPMPay.Services;
using System.Globalization;
using System.Text;

namespace BTPayPro.CPMPay.Reports
{
    public class CPMPayReportGenerator
    {
       // Existing method to generate a list of AccountingEntry objects
        public List<AccountingEntry> GenerateReport(List<FileDetailRecord> detailRecords)
        {
            var reportEntries = new List<AccountingEntry>();

            foreach (var detailRecord in detailRecords)
            {
                reportEntries.Add(MapDetailRecordToAccountingEntry(detailRecord));
            }

            return reportEntries;
        }

        // New method to generate a Markdown report from DetailRecord objects
        public string GenerateMarkdownReports(List<FileDetailRecord> detailRecords)
        {
            if (detailRecords == null || detailRecords.Count == 0)
            {
                return "# Rapport Comptable CPMPAY\n\nAucun enregistrement de détail trouvé.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Rapport Comptable CPMPAY\n");
            sb.AppendLine("Ce rapport présente les détails des transactions CPMPAY.\n");
            sb.AppendLine("| Numéro de Séquence | Code Opération | Date Traitement | Montant Autorisé | Montant Règle | Nom Marchand | Ville Marchand | Statut Opération |");
            sb.AppendLine("|--------------------|----------------|-----------------|------------------|---------------|--------------|----------------|------------------|");

            foreach (var record in detailRecords)
            {
                string operationStatus = GetOperationStatusDescription(record.OperationCode);

                sb.AppendLine($"| {record.RecordSequenceNumber} | {record.OperationCode} | {FormatDate(record.ProcessingDate)} | {FormatAmount(record.TransactionAuthorizedAmount)} | {FormatAmount(record.TransactionSettlementAmount)} | {record.MerchantName.Trim()} | {record.MerchantCity.Trim()} | {operationStatus} |");
            }

            return sb.ToString();
        }

        private AccountingEntry MapDetailRecordToAccountingEntry(FileDetailRecord detailRecord)
        {
            DateTime ParseDate(string dateString)
            {
                if (DateTime.TryParseExact(dateString, "ddMMyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
                return DateTime.MinValue;
            }

            return new AccountingEntry
            {
                RecordSequenceNumber = detailRecord.RecordSequenceNumber,
                OperationCode = detailRecord.OperationCode,
                ProcessingDate = ParseDate(detailRecord.ProcessingDate),
                MerchantName = detailRecord.MerchantName.Trim(),
                MerchantCity = detailRecord.MerchantCity.Trim(),
                AuthorizedAmount = CPMPayParser.ParseAmount(detailRecord.TransactionAuthorizedAmount),
                CurrencyCode = detailRecord.TransactionCurrencyCode,
                SettlementAmount = CPMPayParser.ParseAmount(detailRecord.TransactionSettlementAmount),
                IssuerBankIdentification = detailRecord.IssuerBankIdentification,
                AcquirerBankIdentification = detailRecord.AcquirerBankIdentification,
                AcquirerReferenceNumber = detailRecord.AcquirerReferenceNumber,
                TransactionTypeCode = detailRecord.TransactionTypeCode,
                SecondPresentmentIndicator = detailRecord.SecondPresentmentIndicator
            };
        }

        private string FormatAmount(string amountString)
        {
            if (string.IsNullOrEmpty(amountString)) return "0.000";
            if (amountString.Length <= 3) return "0." + amountString.PadLeft(3, '0');
            return amountString.Insert(amountString.Length - 3, ".");
        }

        private string FormatDate(string dateString)
        {
            if (DateTime.TryParseExact(dateString, "ddMMyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date.ToString("yyyy-MM-dd");
            }
            return dateString;
        }

        private string GetOperationStatusDescription(string operationCode)
        {
            return operationCode switch
            {
                "01" => "Purchase",
                "02" => "Cash Out Branch",
                "03" => "Refunds",
                "04" => "ATM Withdrawal",
                "05" => "Cash In",
                "06" => "Money Transfer",
                "07" => "Bill Payment",
                "11" => "Chargeback Purchase",
                "14" => "Chargeback ATM Withdrawal",
                "16" => "Chargeback Money Transfer",
                _ => "Unknown Status"
            };
        }
    }
}
