using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class UserWhiteBoardVM
    {
        public int uwbId { get; set; }
        public string usName { get; set; }
        public int usrId { get; set; }
        public string uwbName { get; set; }
        public string uwbDetail { get; set; }
        public string uwbDay { get; set; }
        public string uwbTime { get; set; }
        public bool Status { get; set; }
    }
}
