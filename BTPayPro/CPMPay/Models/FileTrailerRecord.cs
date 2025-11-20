namespace BTPayPro.CPMPay.Models
{
    public class FileTrailerRecord
    {
        public string FileTrailerCode { get; set; } // Offset 1, Length 2, '99'
        public string RecordSequenceNumber { get; set; } // Offset 3, Length 6
        public string Filler1 { get; set; } // Offset 9, Length 2
        public string ProcessingDate { get; set; } // Offset 11, Length 6, DDMMYY
        public string DestinationBankIdentification { get; set; } // Offset 17, Length 5
        public string GrandTotalSettlementAmountInDebitId { get; set; } // Offset 22, Length 12, 9(9)V9(3)
        public string GrandTotalSettlementAmountInCreditId { get; set; } // Offset 34, Length 12, 9(9)V9(3)
        public string Filler2 { get; set; } // Offset 46, Length 454, SPACES
        public string EndOfRecord { get; set; } // Offset 500, Length 1, 'X'

        
    }
}
