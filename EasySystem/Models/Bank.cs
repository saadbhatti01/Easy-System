using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySystem.Models
{
    [Table("Bank")]
    public class Bank
    {
        [Key]
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int CountryId { get; set; }
        public bool Status { get; set; }
    }
}
