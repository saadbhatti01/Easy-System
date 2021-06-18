using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasySystem.Models
{
    [Table("Transections")]
    public class Transections
    {
        [Key]
        public int tId { get; set; }
        public string tNarration { get; set; }
        public DateTime tDate { get; set; }
        public DateTime tDateTime { get; set; }
        public int tPaying { get; set; }
        public int tReceiving { get; set; }
        public double TuitionAmount { get; set; }
        public double SoftServiceCharges { get; set; }
        public double ThirdPartyCharges { get; set; }
        public string tStatus { get; set; }
        public string tNumber { get; set; }
    }
}
