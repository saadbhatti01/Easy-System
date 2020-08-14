using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserPayment")]
    public class UserPayment
    {
        [Key]
        public int upId { get; set; }
        public string upDetail { get; set; }
        public DateTime pDate { get; set; }
        public double upAmount { get; set; }
        public string upStatus { get; set; }
        public string upImage { get; set; }
        public int usrId { get; set; }
        public int udrId { get; set; }
        public int ubId { get; set; }
    }
}
