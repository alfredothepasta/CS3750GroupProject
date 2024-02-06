using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSEarlyBird.ViewModels
{
    public class RegisterCourseViewModel
    {
        public int Id {get; set;}
        public string CourseName { get; set; } = string.Empty;
        public string CourseNumber { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRegistered { get; set; } 
    }
}