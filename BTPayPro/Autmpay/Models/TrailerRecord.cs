namespace BTPayPro.Autmpay.Models
{
    public class TrailerRecord
    {
        public string FileTrailerCode { get; set; } // Offset 1, Length 2
        public string RecordSequenceNumber { get; set; } // Offset 3, Length 6
        public string Filler1 { get; set; } // Offset 9, Length 2
        public string ProcessingDate { get; set; } // Offset 11, Length 6
        public string DestinationBankIdentification { get; set; } // Offset 17, Length 5
        public string GrandTotalSettlementAmountAuthorizedInDebit { get; set; } // Offset 22, Length 12
        public string GrandTotalSettlementAmountAuthorizedInCredit { get; set; } // Offset 34, Length 12
        public string Filler2 { get; set; } // Offset 46, Length 554
        public string EndOfRecord { get; set; } // Offset 600, Length 1
    }
}
