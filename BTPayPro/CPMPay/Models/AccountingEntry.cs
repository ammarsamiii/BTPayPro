namespace BTPayPro.CPMPay.Models
{
    public class AccountingEntry
    {
        public string RecordSequenceNumber { get; set; }
        public string OperationCode { get; set; }
        public DateTime ProcessingDate { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCity { get; set; }
        public decimal AuthorizedAmount { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SettlementAmount { get; set; }
        public string IssuerBankIdentification { get; set; }
        public string AcquirerBankIdentification { get; set; }
        public string AcquirerReferenceNumber { get; set; }
        public string TransactionTypeCode { get; set; }
        public string SecondPresentmentIndicator { get; set; }
    }
}
