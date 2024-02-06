using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSEarlyBird.ViewModels
{
    public class RegistrationViewModel
    {
        public List<RegisterCourseViewModel> Courses { get; set; }
        public List<string> DepartmentNames { get; set; }
        public string? Search { get; set; }

        public string? SelectedDept { get; set; }
    }
}