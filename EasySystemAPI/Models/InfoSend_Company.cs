using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("InfoSend_Company")]
    public class InfoSend_Company
    {
        [Key]
        public int cInfoId { get; set; }
        public string cInfoText { get; set; }
        public string cInfoBy { get; set; }
        public int cInfoTo { get; set; }
        public DateTime cInfoDate { get; set; }
        public string cInfoStatus { get; set; }
    }
}
