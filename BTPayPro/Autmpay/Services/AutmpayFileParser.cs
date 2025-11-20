using BTPayPro.Autmpay.Models;
using BTPayPro.Autmpay.Parsers;
using System.IO;

namespace BTPayPro.Autmpay.Services
{
    public class AutmpayFileParser
    {
        private readonly HeaderRecordParser _headerParser;
        private readonly DetailRecordParser _detailParser;
        private readonly TrailerRecordParser _trailerParser;

        public AutmpayFileParser()
        {
            {
                _headerParser = new HeaderRecordParser();
                _detailParser = new DetailRecordParser();
                _trailerParser = new TrailerRecordParser();
            }
        }

        public async Task<AutmpayFile> ParseFile(Stream stream, CancellationToken ct)
        {
            var autmpayFile = new AutmpayFile();
            
            using var reader = new StreamReader(stream);
            string? line;
           
            while ((line = reader.ReadLine()) != null)
            {
                if (ct.IsCancellationRequested)
                    break;

                if (string.IsNullOrWhiteSpace(line)) continue;

                string recordType = line.Substring(0, 2);

                switch (recordType)
                {
                    case "01":
                        autmpayFile.Header = _headerParser.Parse(line);
                        break;
                    case "40":
                        autmpayFile.Details.Add(_detailParser.Parse(line));
                        break;
                    case "99":
                        autmpayFile.Trailer = _trailerParser.Parse(line);
                        break;
                    default:
                        Console.WriteLine($"Type d'enregistrement inconnu: {recordType} dans la ligne: {line}");
                        break;
                }

            }
            return autmpayFile;
        }
            
        
    }

    public class AutmpayFile
    {
        public HeaderRecord Header { get; set; }
        public List<DetailRecord> Details { get; set; }
        public TrailerRecord Trailer { get; set; }

        public AutmpayFile()
        {
            Details = new List<DetailRecord>();
        }
    }

}
