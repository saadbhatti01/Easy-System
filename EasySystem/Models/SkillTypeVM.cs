using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class SkillTypeVM
    {
        public int StId { get; set; }
        public string StName { get; set; }
        public string StDetail { get; set; }
        public bool StStatus { get; set; }
        public string StImage { get; set; }
        public string StCoverImage { get; set; }
        public string StCategory { get; set; }
        public int SubType { get; set; }
        public string SubTypeName { get; set; }
    }
}
