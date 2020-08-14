using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    [Table("UserBankInfo")]
    public class UserBankInfo
    {
        public int ubId { get; set; }
        public int BankId { get; set; }
        public int usrId { get; set; }
        public string ubTitle { get; set; }
        public string ubNumber { get; set; }
        public string ubDetail { get; set; }
        public string Status { get; set; }
        public virtual Users GetUsers { get; set; }
        public virtual Bank GetBank { get; set; }

    }
}
