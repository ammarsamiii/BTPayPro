namespace BTPayPro.CPMPay.Models
{
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
