using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class BankVm
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public bool Status { get; set; }
    }
}
