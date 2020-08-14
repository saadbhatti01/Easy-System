using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserSkills")]
    public class UserSkills
    {
        [Key]
        public int usId { get; set; }
        public int StId { get; set; }
        //public string usName { get; set; }
        public string usDetail { get; set; }
        public int usLevel { get; set; }
        public int usrId { get; set; }
        public bool Status { get; set; }

    }
}
