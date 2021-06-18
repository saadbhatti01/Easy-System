using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySystem.Models
{
    [Table("UserLostTrainee")]
    public class UserLostTrainee
    {
        [Key]
        public int ultId { get; set; }
        public int usrIdM { get; set; }
        public int usrIdC { get; set; }
        public DateTime ultDate { get; set; }
        public string ultReason { get; set; }
        public string ultStatus { get; set; }
    }
}
