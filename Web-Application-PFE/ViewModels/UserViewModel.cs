using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Application_PFE.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string? Nom { get; set; }

        [Required]
        public string? Prenom { get; set; }
        [Required]
        public string? Role { get; set; }
        [Required]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Photo de profil")]
        public string? ExistingImagePath { get; set; }





    }
}
