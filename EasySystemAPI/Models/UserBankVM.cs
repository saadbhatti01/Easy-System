using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class UserBankVM
    {
        public int ubId { get; set; }
        public string BankName { get; set; }
        public int usrId { get; set; }
        public string ubTitle { get; set; }
        public string ubNumber { get; set; }
        public string ubDetail { get; set; }
        public bool Status { get; set; }
    }
}
