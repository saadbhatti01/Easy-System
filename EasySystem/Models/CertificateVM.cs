using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class CertificateVM
    {
        public int ucId { get; set; }
        public string StName { get; set; }
        public string ucNumber { get; set; }
        public string uqrStatus { get; set; }
        public DateTime uqrDate { get; set; }
    }
}
