using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySystem.Models
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
        public virtual Users GetUsers { get; set; }
    }
}
