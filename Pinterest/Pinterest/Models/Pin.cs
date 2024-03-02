using System.ComponentModel.DataAnnotations;
using static Humanizer.On;

namespace Pinterest.Models
{
    public class Pin
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required!")]
        [StringLength(20, ErrorMessage = "The title can't be longer than 20 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "The description is required!")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The content is required!")]
        public string? EmbeddedContentPath { get; set; }

        public DateTime? Date { get; set; }
        public int? LikesCount { get; set; }

        // Conexiunea cu celelalte tabele
        public virtual ICollection<PinCategory>? PinCategories { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }

        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}