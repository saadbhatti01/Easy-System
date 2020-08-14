using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class BlogVM
    {
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
        public int usrId { get; set; }
        public string usrName { get; set; }
    }

}
