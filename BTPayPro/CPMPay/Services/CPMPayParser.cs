
using BTPayPro.CPMPay.Models;
using BTPayPro.CPMPay.Parsers;

namespace BTPayPro.CPMPay.Services
{
    public class CPMPayParser
    {
        private readonly FileHeaderRecordParser _fileHeaderParser;
        private readonly FileDetailRecordParser _fileDetailParser;
        private readonly FileTrailerRecordParser _FileTrailerParser;
        public CPMPayParser()
        {
            _fileHeaderParser = new FileHeaderRecordParser();
            _fileDetailParser = new FileDetailRecordParser();
            _FileTrailerParser = new FileTrailerRecordParser();
        }
        public CPMPayFile ParseCpmFile(Stream fileStream, CancellationToken ct)
        {
            var cpmpayFile = new CPMPayFile();
            using (var reader = new StreamReader(fileStream))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    if (line.Length != 500)
                    {
                        // Log or handle error for malformed line
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string recordType = line.Substring(0, 2);

                    switch (recordType)
                    {
                        case "01":
                            cpmpayFile.Headers = _fileHeaderParser.Parse(line);
                            break;
                        case "40":
                            cpmpayFile.Detail.Add(_fileDetailParser.Parse(line));
                            break;
                        case "99":
                            cpmpayFile.Trailer = _FileTrailerParser.Parse(line);
                            break;
                        default:
                            Console.WriteLine($"Type d'enregistrement inconnu: {recordType} dans la ligne: {line}");
                            break;
                    }
                }

            }
            return cpmpayFile;


        }

        public static decimal ParseAmount(string amountString)
        {
            // Amounts are in 9(9)V9(3) format, meaning 9 digits before decimal and 3 after.
            // The decimal point is implicit. E.g., "00000003287" means 3.287
            if (string.IsNullOrWhiteSpace(amountString) || amountString.Length != 12)
            {
                return 0m; // Or throw an exception, depending on desired error handling
            }

            if (!long.TryParse(amountString, out long rawValue))
            {
                return 0m; // Or throw an exception
            }

            // Divide by 1000 to get the correct decimal value
            return rawValue / 1000m;
        }
        public class CPMPayFile
        {
            public FileHeaderRecord Headers { get; set; }
            public List<FileDetailRecord> Detail { get; set; }
            public FileTrailerRecord Trailer { get; set; }
            public CPMPayFile()
            {
                Detail = new List<FileDetailRecord>();
            }
        }

    }
}
 
