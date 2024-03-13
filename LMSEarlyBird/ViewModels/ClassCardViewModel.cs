using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSEarlyBird.ViewModels
{
    public class ClassCardViewModel
    {
        public int CourseId {get; set; } 
        public string DeptCode { get; set; } = string.Empty;
        public string CourseNumber {get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string DaysOfWeek { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
    }
}