using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserHelpingMaterial")]
    public class UserHelpingMaterial
    {
        [Key]
        public int umsId { get; set; }
        public string umsTitle { get; set; }
        public string umsPath { get; set; }
        public string umsType { get; set; }
        public string umsFontName { get; set; }
        public string umsFontColour { get; set; }
        public int umsFontSize { get; set; }
        public string umsFontAlignment { get; set; }
        public string umsFor { get; set; }
        public bool umsStatus { get; set; } 
        public DateTime umsCreatedDate { get; set; }
        public int umsCreatedBy { get; set; }
    }
}
