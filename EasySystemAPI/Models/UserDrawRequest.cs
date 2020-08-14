using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserDrawRequest")]
    public class UserDrawRequest
    {
        [Key]
        public int udrId { get; set; }
        public string udrDetail { get; set; }
        public string udrCode { get; set; }
        public DateTime udrCreatedDate { get; set; }
        public double udrAmount { get; set; }
        public DateTime udrActionDate { get; set; }
        public string  udrRemarks { get; set; }
        public string udrStatus { get; set; }
        public string udrImage { get; set; }
        public int usrId { get; set; } 
        public int ubId { get; set; }
    }
}
