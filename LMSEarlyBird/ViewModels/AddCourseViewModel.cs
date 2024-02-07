using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.ViewModels
{
    public class AddCourseViewModel
    {
        [Required]
        public int Department { get; set; }
        public List<Department>? DepartmentList { get; set; }
        [Required]
        [Display(Name = "Course Number")]
		public string CourseNumber { get; set; }
        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
        [Required]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }
        [Required]
        [Display(Name = "Start Time")]
        public TimeOnly StartTime { get; set; }
        [Required]
        [Display(Name = "End Time")]
        public TimeOnly EndTime { get; set; }
        public string DayOfWeekM { get; set; }
        public string DayOfWeekT { get; set; }
        public string DayOfWeekW { get; set; }
        public string DayOfWeekR { get; set; }
        public string DayOfWeekF { get; set; }
        [Required]
        public int Building { get; set; }
        public List<Building>? BuildingList { get; set; }
        [Required]
        [Remote(action: "roomAvailability", controller: "Instructor", 
            AdditionalFields = nameof(StartTime) + "," + 
            nameof(EndTime) + "," + 
            nameof(DayOfWeekM) + "," + 
            nameof(DayOfWeekT) + "," + 
            nameof(DayOfWeekW) + "," + 
            nameof(DayOfWeekR) + "," + 
            nameof(DayOfWeekF)
			)]
        public int Room { get; set; }
        public List<Room>? RoomList { get; set; }
    }
}