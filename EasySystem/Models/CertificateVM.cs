using System;

namespace EasySystem.Models
{
    public class CertificateVM
    {
        public int ucId { get; set; }
        public string StName { get; set; }
        public string ucNumber { get; set; }
        public string uqrStatus { get; set; }
        public string usrName { get; set; }
        public string Mentor { get; set; }
        public DateTime uqrDate { get; set; }
        public DateTime ucDate { get; set; }
    }
}
