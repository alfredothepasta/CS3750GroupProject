using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class SubmissionViewModel
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxPoints { get; set; }
        public string Type {get; set;} = string.Empty;
        public string FileName {get; set;} = string.Empty;
        public string StudentId {get; set;} = string.Empty;
        public int CourseId {get; set;}
        public bool Submitted = false;
        public bool Late {get; set;}
        public string SubmissionDate {get; set;}
        public bool Graded {get; set;}
        public int Score {get; set;}

        public string SubmissionComment {get; set;} = string.Empty;

        public string DueDate {get; set;} = string.Empty;

        [Required(ErrorMessage = "Must provide text")]
        [MinLength(10, ErrorMessage = "Submission text must be at least 10 characters long")]
        [RegularExpression(".{10,}", ErrorMessage = "Submission text must be at least 10 characters long")]
        public string SubmissionTxt { get; set; } = string.Empty;

        [Required(ErrorMessage = "Must provide a file")]
        public IFormFile? File { get; set; }
    }
}