using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class PaymentHistory
    {
        public int Id { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }

        public AppUser AppUser { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        public DateTime PaymentTime { get; set; }
    }
}
