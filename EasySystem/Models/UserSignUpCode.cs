using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    [Table("UserSignUpCode")]
    public class UserSignUpCode
    {
        [Key]
        public int uscId { get; set; }
        public int uscCode { get; set; }
        public DateTime uscExpire { get; set; }
        public string usEmail { get; set; }
        public string usPhone { get; set; }
        public string uscStatus { get; set; }
    }
}
