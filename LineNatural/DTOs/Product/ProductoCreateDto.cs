using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineNatural.Entities
{
    public class ProductoCreateDto
    {
        [Required]
        [Range(1, Int32.MaxValue - 1)]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(30)]
        public string ProductName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0.0,999.99)]
        public double Price { get; set; }
        [Required]
        [Range(0,100)]
        public int Stock { get; set; }
    }
}
