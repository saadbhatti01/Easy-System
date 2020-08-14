using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class QuestionVM
    {
        public int qId { get; set; }
        public string qQuestion { get; set; }
        public string qOpt1 { get; set; }
        public string qOpt2 { get; set; }
        public string qOpt3 { get; set; }
        public string qOpt4 { get; set; }
        public string qAnswer { get; set; }
        public int StId { get; set; }
        public string StName { get; set; }
        public int qCategory { get; set; }
        public bool qStatus { get; set; }
    }
}
