using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class InfoSendVM
    {
        public int infoId { get; set; }
        public string infoText { get; set; }
        public string infoBy { get; set; }
        public int infoTo { get; set; }
        public DateTime infoDate { get; set; }
        public string infoStatus { get; set; }
        public string User { get; set; }
        public string Code { get; set; }
    }
}
