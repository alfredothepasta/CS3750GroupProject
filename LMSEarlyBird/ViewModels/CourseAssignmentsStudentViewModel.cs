using LMSEarlyBird.Models;

namespace LMSEarlyBird.ViewModels
{
    public class CourseAssignmentsStudentViewModel
    {
        public Course Course { get; set; }
        public double Grade { get; set; }
        public string LetterGrade { get; set; }
        public List<StudentAssignment>? Assignments { get; set; }


        public List<StudentAssignment> StudentAssignment { get; set; } = new List<StudentAssignment>();
        public int numA { get; set; }
        public int numAm { get; set; }
        public int numBp { get; set; }
        public int numB { get; set; }
        public int numBm { get; set; }
        public int numCp { get; set; }
        public int numC { get; set; }
        public int numCm { get; set; }
        public int numDp { get; set; }
        public int numD { get; set; }
        public int numDm { get; set; }
        public int numE { get; set; }
        public int numF { get; set; }
    }
}