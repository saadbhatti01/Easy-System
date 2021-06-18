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
        public double IntTuitionFee { get; set; }
        public double IntSoftwdareFee { get; set; }
        public double IntThirdPartyFee { get; set; }
        public double IntTotal { get; set; }
        public double IntDrawAmount { get; set; }
        public double IntSignUpAmount { get; set; }
        public string CompanyMail { get; set; }
        public string MailMessage { get; set; }
    }
}
