using System.ComponentModel.DataAnnotations;

namespace AIDMS.DTOs
{
    public class NationalIdDto
    {
        [Required(ErrorMessage = "National ID is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be numeric and 14 digits long")]
        public string NationalID { get; set; }
    }
}
