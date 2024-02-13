using LMSEarlyBird.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class CreateAssignmentViewModel
    {
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
        [Display(Name = "Max Points")]
		public int maxPoints { get; set; }
    }
}
