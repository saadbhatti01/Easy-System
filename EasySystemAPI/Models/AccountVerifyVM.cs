using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class AccountVerifyVM
    {
        public int uvId { get; set; }
        public int uvtId { get; set; }
        public int usrId { get; set; }
        public int uvStatusBy { get; set; }
        public DateTime uvCreatedDate { get; set; }
        public string uvStatus { get; set; }
        public DateTime uvStatusDate { get; set; }
        public string uvStatusRemarks { get; set; }
        public string uvImagePath { get; set; }
        public string usrName { get; set; }
        public string Admin { get; set; }
        public string uvtName { get; set; }
        public string uvText { get; set; }
    }
}
