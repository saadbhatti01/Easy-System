using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("Coupan")]
    public class Coupan
    {
        [Key]
        public int cId { get; set; }
        public int cCode { get; set; }
        public double cAmount { get; set; }
        public bool cStatus { get; set; }
        public DateTime cCreatedDate { get; set; }
        public int cCreatedBy { get; set; }
        public DateTime cExpiryDate { get; set; }
        public int cAssignedUserId { get; set; }
        public int cUsedUserId { get; set; }
        public DateTime cUsedDate { get; set; }
    }
}
