using BTPayPro.CPMPay.Models;
using BTPayPro.CPMPay.Services;

namespace BTPayPro.CPMPay.Parsers
{
    public class FileTrailerRecordParser : IFileRecordParser<FileTrailerRecord>
    {
        public FileTrailerRecord Parse(string line)
        {
            if (line.Length != 500)
            {
                throw new ArgumentException("Record line must be 500 characters long.");
            }

            return new FileTrailerRecord
            {
                FileTrailerCode = line.Substring(0, 2),
                RecordSequenceNumber = line.Substring(2, 6),
                Filler1 = line.Substring(8, 2),
                ProcessingDate = line.Substring(10, 6),
                DestinationBankIdentification = line.Substring(16, 5),
                GrandTotalSettlementAmountInDebitId = line.Substring(21, 12),
                GrandTotalSettlementAmountInCreditId = line.Substring(33, 12),
                Filler2 = line.Substring(45, 454),
                EndOfRecord = line.Substring(499, 1)
            };
        }
    }
}
