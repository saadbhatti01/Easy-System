using System;

namespace EasySystem.Models
{
    public class TeamVM
    {
        public int usrId { get; set; }
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
        public string usrCity { get; set; }
        public int memId { get; set; }
        public int refId { get; set; }
        public int roleId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime LogDateTime { get; set; }
    }
}
