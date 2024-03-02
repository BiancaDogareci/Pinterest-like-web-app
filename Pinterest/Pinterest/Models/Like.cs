using System.ComponentModel.DataAnnotations;

namespace Pinterest.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        public int? PinId { get; set; }
        public virtual Pin Pin { get; set; }
        public string? AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }


    }
}