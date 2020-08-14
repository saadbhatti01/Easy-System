using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class Dashboard
    {
        public int Active { get; set; }
        public int Learner { get; set; }
        public int Lost { get; set; }
        public double Current { get; set; }
        public double Withdraw { get; set; }
        public double Total { get; set; }
    }

}
