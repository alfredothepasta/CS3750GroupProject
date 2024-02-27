using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSEarlyBird.ViewModels
{
    public class AssigmentGradeViewModel
    {
        public string AssignmentName {get; set;} = string.Empty;
        public bool Submitted {get; set;}
        public string StudentName {get; set;} = string.Empty;
        public string DueDate {get; set;} = string.Empty;
        public string SubmissionDate {get; set;} = string.Empty;
        public bool LateSubmission {get; set;}
        public bool Graded { get; set; }
        public int GradedPoints { get; set; }
        public int MaxPoints {get; set;}
        
        public bool FileSubmission {get; set;}
        public string FileName {get; set;} = string.Empty;
        public string SubmissionComment {get; set;} = string.Empty;

        public string StudentId { get; set; }
        public int CourseId { get; set; }
        public int AssignmentId { get; set; }
    }
}