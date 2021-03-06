using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace EasySystemAPI.Models
{
    [Table("Blog")]
    public class Blog
    {
        [Key]
        public int blogId { get; set; }
        public string blogTitle { get; set; }
        //[AllowHtml]
        //[UIHint("tinymce_full")]
        public string blogDescription { get; set; }
        public string blogTitleImage { get; set; }
        public string blogCoverPageImage { get; set; }
        public string blogStatus { get; set; }
        public int blogType { get; set; }
        public DateTime createdDate { get; set; }
        public int UsrId { get; set; }
    }
}