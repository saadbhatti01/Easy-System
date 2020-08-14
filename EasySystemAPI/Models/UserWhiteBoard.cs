using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserWhiteBoard")]
    public class UserWhiteBoard
    {
        [Key]
        public int uwbId { get; set; }
        public int usId { get; set; }
        public int usrId { get; set; }
        public string uwbName { get; set; }
        public string uwbDetail { get; set; }
        public string uwbDay { get; set; }
        public string uwbTime { get; set; }
        public bool Status { get; set; }
    }
}
