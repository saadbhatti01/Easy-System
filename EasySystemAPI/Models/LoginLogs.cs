using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("LoginLogs")]
    public class LoginLogs
    {
        [Key]
        public int LogId { get; set; }
        public int UsrId { get; set; }
        public int MentorId { get; set; }
        public DateTime LogDateTime { get; set; }
    }
}
