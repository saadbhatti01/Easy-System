using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UsersChat")]
    public class UsersChat
    {
        [Key]
        public int ucId { get; set; }
        public string ucContent { get; set; }
        public int ucDateTime { get; set; }
        public int ucSenderId { get; set; }
        public int ucReceiverId { get; set; }
    }
}
