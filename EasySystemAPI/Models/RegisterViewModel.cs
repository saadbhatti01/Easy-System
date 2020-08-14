using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystemAPI.Models
{
    public class RegisterViewModel
    {
        public string usrEmail { get; set; }
        public string usrPhone { get; set; }
        public string usrPassword { get; set; }
        public string usrName { get; set; }
        public string usrGender { get; set; }
        public int usrCode { get; set; }
        public string usrImage { get; set; }
        public string usrStatus { get; set; }
        public DateTime usrCreated { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string usrCity { get; set; }
        public int memId { get; set; }
        public int refId { get; set; }
        public string Mentor { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
