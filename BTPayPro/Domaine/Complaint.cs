using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTPayPro.Domaine
{
    [Table("Complaints")]
    public class Complaint
    {
        [Key]

        public string ComplaintId { get; set; }
        public string Description { get; set; }
        public DateOnly DateComplaint { get; set; }
        public string ComplaintStatus { get; set; }
        public string Channel { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }

}
