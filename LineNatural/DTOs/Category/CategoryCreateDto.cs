using System.ComponentModel.DataAnnotations;

namespace LineNatural.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required]
        [MaxLength(30)]
        public string CategoryName { get; set; }
    }
}
