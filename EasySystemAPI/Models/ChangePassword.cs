using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class ChangePassword
    {
        public int Id { get; set; }
        public string cPassword { get; set; }
        public string nPassword { get; set; }
        public string rPassword { get; set; }
    }
}
