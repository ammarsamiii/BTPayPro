using BTPayPro.Autmpay.Models;
using BTPayPro.Autmpay.Services;

namespace BTPayPro.Autmpay.Parsers
{
    public class TrailerRecordParser : IRecordParser<TrailerRecord>
    {
        public TrailerRecord Parse(string line)
        {
            return new TrailerRecord
            {
                FileTrailerCode = line.Substring(0, 2),
                RecordSequenceNumber = line.Substring(2, 6),
                Filler1 = line.Substring(8, 2),
                ProcessingDate = line.Substring(10, 6),
                DestinationBankIdentification = line.Substring(16, 5),
                GrandTotalSettlementAmountAuthorizedInDebit = line.Substring(21, 12),
                GrandTotalSettlementAmountAuthorizedInCredit = line.Substring(33, 12),
                Filler2 = line.Substring(45, 554),
                EndOfRecord = line.Substring(599, 1)
            };
        }
    }
}
