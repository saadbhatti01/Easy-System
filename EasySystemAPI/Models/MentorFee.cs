using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("MentorFee")]
    public class MentorFee
    {
        [Key]
        public int mfId { get; set; }
        public int usrId { get; set; }
        public double usrFeeAmount { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
