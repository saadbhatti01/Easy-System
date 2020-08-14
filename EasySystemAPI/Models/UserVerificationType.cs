using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserVerificationType")]
    public class UserVerificationType
    {
        [Key]
        public int uvtId { get; set; }
        public string uvtName { get; set; }
        public string uvtText { get; set; }
        public bool uvtStatus { get; set; }

    }
}
