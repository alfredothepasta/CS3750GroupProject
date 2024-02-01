using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }
        public string BuildingName { get; set; }

    }
}
