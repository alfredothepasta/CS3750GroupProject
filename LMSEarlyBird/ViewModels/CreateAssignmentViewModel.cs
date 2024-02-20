using LMSEarlyBird.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class CreateAssignmentViewModel
    {
        public int AssignmentId { get; set; }
		public Course? Course { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Submission Type")]
		[StringLength(4)]
        [RegularExpression("^(?:file|text)$")]
        public string Type { get; set; }
        [Required]
        [Display(Name = "Due Date")]
		public DateTime DueDate { get; set; }
        [Required]
		[Range(0, int.MaxValue, ErrorMessage = "The field {0} must be within {1} and {2}")]
		[Display(Name = "Max Points")]
		public int maxPoints { get; set; }
    }
}
