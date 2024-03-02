using System.ComponentModel.DataAnnotations;

namespace Pinterest.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The content is required!")]
        public string? Text { get; set; }

        public DateTime? Date { get; set; }

        // Conexiunea cu celelalte tabele:
        public int? PinId { get; set; }
        public virtual Pin? Pin { get; set; }
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
