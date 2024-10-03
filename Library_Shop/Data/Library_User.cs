using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Library_Shop.Data
{
    public class Library_User : IdentityUser
    {
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}
