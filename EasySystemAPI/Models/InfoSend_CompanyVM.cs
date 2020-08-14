using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class InfoSend_CompanyVM
    {
        public int cInfoId { get; set; }
        public string cInfoText { get; set; }
        public string cInfoBy { get; set; }
        public int cInfoTo { get; set; }
        public DateTime cInfoDate { get; set; }
        public string cInfoStatus { get; set; }
        public string User { get; set; }
        public string Code { get; set; }
    }
}
