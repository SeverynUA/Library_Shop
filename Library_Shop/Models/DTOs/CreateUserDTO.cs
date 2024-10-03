using System.ComponentModel.DataAnnotations;

namespace Library_Shop.Models.DTOs
{
    public class CreateUserDTO
    {

        [Display(Name = "Login")]
        [Required]
        public string Login { get; set; } = default!;

        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Display(Name = "Date Of Birth")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
