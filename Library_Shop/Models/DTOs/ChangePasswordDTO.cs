using System.ComponentModel.DataAnnotations;

namespace Library_Shop.Models.DTOs
{
    public class ChangePasswordDTO
    {
        public string Id { get; set; } = default!;
        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Display(Name = "Old password")]
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = default!;
        [Display(Name = "New password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = default!;
    }
}
