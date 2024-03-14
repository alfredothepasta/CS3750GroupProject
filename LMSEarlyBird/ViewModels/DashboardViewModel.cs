using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public List<StudentAssignment> StudentAssignment { get; set; } = new List<StudentAssignment>();
       
        public List<ClassCardViewModel> ClassCards { get; set; } = new List<ClassCardViewModel>();
    }
}
