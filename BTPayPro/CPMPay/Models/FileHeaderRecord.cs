namespace BTPayPro.CPMPay.Models
{
    public class FileHeaderRecord
    {
        public string HeaderCode { get; set; } // Offset 1, Length 2, '01'
        public string RecordSequenceNumber { get; set; } // Offset 3, Length 6
        public string Filler1 { get; set; } // Offset 9, Length 2, SPACES
        public string ProcessingDate { get; set; } // Offset 11, Length 6, DDMMYY
        public string InternalRecordCode { get; set; } // Offset 17, Length 6, '222222'
        public string DestinationBankIdentification { get; set; } // Offset 23, Length 5
        public string Filler2 { get; set; } // Offset 28, Length 472, SPACES
        public string EndOfRecord { get; set; } // Offset 500, Length 1, 'X'

        
    }
}
