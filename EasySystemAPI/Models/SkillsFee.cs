using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class SkillsFee
    {
        public double TuitionFee { get; set; }
        public double SoftwdareFee { get; set; }
        public double ThirdPartyFee { get; set; }
        public double Total { get; set; }
        public double DrawAmount { get; set; }
        public double SignUpAmount { get; set; }
        public string CompanyMail { get; set; }
        public string MailMessage { get; set; }
    }
}
