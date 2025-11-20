namespace BTPayPro.Autmpay.Models
{
    public class HeaderRecord
    {

        public string HeaderCodeRecord { get; set; } // Offset 1, Length 2
        public string RecordSequenceNumber { get; set; } // Offset 3, Length 6
        public string Filler1 { get; set; } // Offset 9, Length 2
        public string ProcessingDate { get; set; } // Offset 11, Length 6
        public string InternalRecordCode { get; set; } // Offset 17, Length 6
        public string DestinationBankIdentification { get; set; } // Offset 23, Length 5
        public string Filler2 { get; set; } // Offset 28, Length 572
        public string EndOfRecord { get; set; } // Offset 600, Length 1

    }
}
