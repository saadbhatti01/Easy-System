using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("InfoSend")]
    public class InfoSend
    {
        [Key]
        public int infoId { get; set; }
        public string infoText { get; set; }
        public string infoBy { get; set; }
        public int infoTo { get; set; }
        public DateTime infoDate { get; set; }
        public string infoStatus { get; set; }
    }
}
