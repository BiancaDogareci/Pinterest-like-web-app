using System.ComponentModel.DataAnnotations.Schema;

namespace Pinterest.Models
{
    public class PinCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? PinId { get; set; }
        public int? CategoryId { get; set; }
        public virtual Pin? Pin { get; set; }
        public virtual Category? Category { get; set; }
        public DateTime CategoryDate { get; set; }
    }
}
