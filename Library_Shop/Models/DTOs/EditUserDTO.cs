using System.ComponentModel.DataAnnotations;

namespace Library_Shop.Models.DTOs
{
    public class EditUserDTO
    {
        public string Id { get; set; } = default!;
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
    }
}
