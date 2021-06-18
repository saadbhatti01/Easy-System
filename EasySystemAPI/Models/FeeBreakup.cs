using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("FeeBreakup")]
    public class FeeBreakup
    {
        [Key]
        public int fbId { get; set; }
        public int fbMentorPer { get; set; }
        public int fbSystemPer { get; set; }
        public int fbThirdPartyPer { get; set; }
        public bool fbStatus { get; set; }
        public DateTime fbAppliedDate { get; set; }
        public DateTime fbEndDate { get; set; }
    }
}
