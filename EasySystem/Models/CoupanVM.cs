using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class CoupanVM
    {
        public int cId { get; set; }
        public int cCode { get; set; }
        public double cAmount { get; set; }
        public bool cStatus { get; set; }
        public DateTime cCreatedDate { get; set; }
        public int cCreatedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime cExpiryDate { get; set; }
        public int cAssignedUserId { get; set; }
        public string AssignedUser { get; set; }
        public int cUsedUserId { get; set; }
        public string UsedUser { get; set; }
        public DateTime cUsedDate { get; set; }
    }
}
