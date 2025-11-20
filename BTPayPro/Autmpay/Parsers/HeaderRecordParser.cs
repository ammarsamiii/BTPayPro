using BTPayPro.Autmpay.Models;
using BTPayPro.Autmpay.Services;

namespace BTPayPro.Autmpay.Parsers
{
    public class HeaderRecordParser : IRecordParser<HeaderRecord>
    {
        public HeaderRecord Parse(string line)
        {
            return new HeaderRecord
            {
                HeaderCodeRecord = line.Substring(0, 2),
                RecordSequenceNumber = line.Substring(2, 6),
                Filler1 = line.Substring(8, 2),
                ProcessingDate = line.Substring(10, 6),
                InternalRecordCode = line.Substring(16, 6),
                DestinationBankIdentification = line.Substring(22, 5),
                Filler2 = line.Substring(27, 572),
                EndOfRecord = line.Substring(599, 1)
            };
        }
    }
}
