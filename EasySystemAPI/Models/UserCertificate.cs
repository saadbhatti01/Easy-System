using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserCertificate")]
    public class UserCertificate
    {
        [Key]
        public int ucId { get; set; }
        public int usrId { get; set; }
        public int uqrId { get; set; }
        public DateTime ucDate { get; set; }
        public string ucNumber { get; set; }
        public int StId { get; set; }
    }
}
