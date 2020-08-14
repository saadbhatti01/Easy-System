using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserVerification")]
    public class UserVerification
    {
        [Key]
        public int uvId { get; set; }
        public int uvtId { get; set; }
        public int usrId { get; set; }
        public int uvStatusBy { get; set; }
        public DateTime uvCreatedDate { get; set; }
        public string uvText { get; set; }
        public string uvStatus { get; set; }
        public DateTime uvStatusDate { get; set; }
        public string uvStatusRemarks { get; set; }
        public string uvImagePath { get; set; }
    }
}
