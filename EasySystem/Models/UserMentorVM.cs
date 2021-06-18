using System;

namespace EasySystem.Models
{
    public class UserMentorVM
    {
        public int MentorId { get; set; }
        public string CoreMentor { get; set; }
        public string MentorName { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateTime LeftDate { get; set; }
    }
}
