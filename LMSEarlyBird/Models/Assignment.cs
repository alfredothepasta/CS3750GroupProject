using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [StringLength(4)]
        [RegularExpression("^(?:file|text)$")]
        public string Type { get; set; }
        public DateTime DueDate { get; set; }
        public int maxPoints { get; set; }
        [ForeignKey("CourseId")]
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}