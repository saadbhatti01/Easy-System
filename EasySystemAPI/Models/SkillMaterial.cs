using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    [Table("SkillMaterial")]
    public class SkillMaterial
    {
        [Key]
        public int SmId { get; set; }
        public int StId { get; set; }
        public string SmTitle { get; set; }
        public string SmContent { get; set; }
        public bool SmStatus { get; set; }
        public int CreatedBy { get; set; }
    }
}
