using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("UserQuestionnaireResult")]
    public class UserQuestionnaireResult
    {
        [Key]
        public int uqrId { get; set; }
        public int usrId { get; set; }
        public DateTime uqrDate { get; set; }
        public string uqrStatus { get; set; }
        public int qCategory { get; set; }
        public int StId { get; set; }
    }
}
