using System.Collections.Generic;





namespace BTPayPro.Autmpay.Models
{
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
