using Microsoft.AspNetCore.Identity;

namespace Web_Application_PFE.Models
{
    public class User : IdentityUser
    {
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? imagePath { get; set; }
    }
}

