using System.ComponentModel.DataAnnotations;

namespace Pinterest.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required!")]
        public string? Name { get; set; }

        // Conexiunea cu celelalte tabele:
        public virtual ICollection<PinCategory>? PinCategories { get; set; }
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
