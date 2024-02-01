using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
    }
}
