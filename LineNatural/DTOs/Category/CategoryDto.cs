using System.ComponentModel.DataAnnotations;

namespace LineNatural.DTOs.Category
{
    public class CategoryDto
    {
        [Required]
        [Range(minimum: 1, maximum: Int32.MaxValue, ErrorMessage = "El identificador no se encuentra dentro de los parametros")]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string CategoryName { get; set; }
    }
}
