using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class UserLinks
    {
        public int Id { get; set; }
        [ForeignKey("AppUser")]
        public string? UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? Link1 { get; set; } = String.Empty;
        public string? Link2 { get; set; } = String.Empty;
        public string? Link3 { get; set; } = String.Empty;
    }
}
