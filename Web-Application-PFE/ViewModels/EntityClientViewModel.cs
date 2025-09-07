using System.ComponentModel.DataAnnotations;
using Web_Application_PFE.Models;

namespace Web_Application_PFE.ViewModels
{
    public class EntityClientViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Sales { get; set; }
        [Required]
        [StringLength(100)]
        public string? Customer { get; set; }

        [Required]
        [StringLength(100)]
        public string? EmailClient { get; set; }

       
    }
}
