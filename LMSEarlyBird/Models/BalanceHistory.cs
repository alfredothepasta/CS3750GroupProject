using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class BalanceHistory
    {
        public int Id { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }

        public AppUser AppUser { get; set; }

        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        [Column(TypeName = "money")]
        public decimal NetChange { get; set; }

        public DateTime Time { get; set; }

        [RegularExpression("^(?:AddClass|DropClass|Payment)$")]
        public string Type { get; set; }
      
        public string? ClassName { get; set; }
    }
}