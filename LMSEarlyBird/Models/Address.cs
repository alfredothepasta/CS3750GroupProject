using System.ComponentModel.DataAnnotations;

namespace LMSEarlyBird.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; } 
        public string LineOne { get; set; } = string.Empty;
        public string LineTwo { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int ZipCode { get; set; }
    }
}