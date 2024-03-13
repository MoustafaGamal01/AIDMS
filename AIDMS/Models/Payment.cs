using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIDMS.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Document URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string DocumentURL { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Timestamp is required")]
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp { get; set; }
        
        
    }
}
