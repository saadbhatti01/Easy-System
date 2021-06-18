namespace EasySystem.Models
{
    public class SkillDetailVM
    {
        public int SmId { get; set; }
        public int StId { get; set; }
        public string SkillName { get; set; }
        public string SmTitle { get; set; }
        public string SmContent { get; set; }
        public bool SmStatus { get; set; }
        public string CreatedBy { get; set; }
    }
}
