using BTPayPro.CPMPay.Models;
using BTPayPro.CPMPay.Services;

namespace BTPayPro.CPMPay.Parsers
{
    public class FileHeaderRecordParser : IFileRecordParser<FileHeaderRecord>
    {
        public FileHeaderRecord Parse(string line)
        {
            if (line.Length != 500)
            {
                throw new ArgumentException("Record line must be 500 characters long.");
            }

            return new FileHeaderRecord
            {
                HeaderCode = line.Substring(0, 2),
                RecordSequenceNumber = line.Substring(2, 6),
                Filler1 = line.Substring(8, 2),
                ProcessingDate = line.Substring(10, 6),
                InternalRecordCode = line.Substring(16, 6),
                DestinationBankIdentification = line.Substring(22, 5),
                Filler2 = line.Substring(27, 472),
                EndOfRecord = line.Substring(499, 1)
            };
        }
    }
}
