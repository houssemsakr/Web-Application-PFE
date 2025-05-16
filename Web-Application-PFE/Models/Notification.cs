using System.ComponentModel.DataAnnotations;

namespace Web_Application_PFE.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserName { get; set; }  // Nom de l'utilisateur qui a créé le RFQ

        [Required]
        public string? UserRole { get; set; }  // Rôle de l'utilisateur

        [Required]
        public DateTime? CreationDate { get; set; } = DateTime.Now;

        [Required]
        public int? RFQId { get; set; } // Numéro du RFQ

        public string? Comment { get; set; }  // Commentaire optionnel

        [Required]
        public string? Status { get; set; }  // Statut de la notification

        [Required]
        public bool? IsRead { get; set; } = false;  // Si la notification a été lue
        [Required]
        public int? CustomRFQNumber { get; set; }

        // Relation avec le RFQ
        public AddRFQ? RFQ { get; set; }
    }
}