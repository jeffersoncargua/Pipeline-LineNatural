using System.ComponentModel.DataAnnotations;

namespace LineNatural.DTOs.LocalUser
{
    public class ForgetRequestDto
    {
        
        [StringLength(30)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-zA-Z0-9!#$%&'*\/=?^_`\{\|\}~\+\-]([\.]?[a-zA-Z0-9!#$%&'*\/=?^_`\{\|\}~\+\-])+@[a-zA-Z0-9]([^@&%$\/\(\)=?¿!\.,:;]|\d)+[a-zA-Z0-9][\.][a-zA-Z]{2,4}([\.][a-zA-Z]{2})?")]
        [Required]
        public string UserName { get; set; }
    }
}
