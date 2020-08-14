using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("Question_Story")]
    public class Question_Story
    {
        [Key]
        public int qId { get; set; }
        public string qQuestion { get; set; }
        public string qOpt1 { get; set; }
        public string qOpt2 { get; set; }
        public string qOpt3 { get; set; }
        public string qOpt4 { get; set; }
        public string qAnswer { get; set; }
        public int StId { get; set; }
        public int qCategory { get; set; }
        public bool qStatus { get; set; }
    }
}
