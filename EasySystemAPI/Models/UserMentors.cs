using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserMentors")]
    public class UserMentors
    {
        [Key]
        public int umId { get; set; }
        public int usrId { get; set; }
        public int mentId { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime deletedDate { get; set; }
        public bool umStatus { get; set; }
    }
}
