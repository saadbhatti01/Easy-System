using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class AppTransaction
    {
        public int Id { get; set; }
        public string From { get; set; }
        public DateTime Date { get; set; }
        public string To { get; set; }
        public string Status { get; set; }
    }
}
