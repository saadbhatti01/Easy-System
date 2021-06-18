using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("SkillMaterialDetail")]
    public class SkillMaterialDetail
    {
        [Key]
        public int SmdId { get; set; }
        public string SmdURL { get; set; }
        public string SmdFilePath { get; set; }
        public int SmId { get; set; }
        public int StId { get; set; }
    }
}
