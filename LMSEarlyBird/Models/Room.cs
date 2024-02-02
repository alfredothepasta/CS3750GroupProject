using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSEarlyBird.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public bool IsAvailable { get; set; }
        public int BuildingID { get; set; }
        public Building Building { get; set; }
    }
}