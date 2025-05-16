using System.ComponentModel.DataAnnotations;
using Web_Application_PFE.Models;

namespace Web_Application_PFE.ViewModels
{
    public class AddRFQViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        [Required]
        [StringLength(200)]
        public string? Sales { get; set; }
        [Required]
        [StringLength(200)]
        public string? Customer { get; set; }
        [Required]
        [StringLength(200)]
        public string? QuoteName { get; set; } // Nom de la RFQ

        [Required]
        [StringLength(100)]
        public string? MarketSegment { get; set; } // Segment de marché

        [Required]
        public int NumberOfRefToBeQuoted { get; set; }
        [Required]
        public int MaxVolume { get; set; }

        [Required]
        public int EstimatedVolume { get; set; }

        [Required]
        public DateTime SOPDate { get; set; } = new DateTime(2025, 1, 1);

        [Required]
        public DateTime KOdate { get; set; } = new DateTime(2025, 1, 1);

        [Required]
        public DateTime CustomerDatadate { get; set; } = new DateTime(2025, 1, 1);

        [Required]
        public string? MaterialLeader { get; set; }

        [Required]
        public DateTime MaterialDueDate { get; set; } = new DateTime(2025, 1, 1);

        [Required]
        public DateTime MaterialRelease { get; set; } = new DateTime(2025, 1, 1);

        [Required]
        public string? TestLeader { get; set; }
        [Required]
        public DateTime TestDueDate { get; set; } = new DateTime(2025, 1, 1);

        [Required]
        public DateTime TestRelease { get; set; } = new DateTime(2025, 1, 1);
        [Required]
        public string? VALeader { get; set; }
        [Required]
        public DateTime LabourDueDate { get; set; } = new DateTime(2025, 1, 1);
        [Required]
        public DateTime LabourRelease { get; set; } = new DateTime(2025, 1, 1);
        [Required]
        public DateTime CustomerDueDate { get; set; } = new DateTime(2025, 1, 1);
        [Required]
        public string? WorkingStatus { get; set; }
        [Required]
        public DateTime ApprovalDate { get; set; } = new DateTime(2025, 1, 1);
     

        public string? CreatorFullName { get; set; }
     
        public string? CreatorId { get; set; }
        public string? Statut { get; set; }
       
        public string? Comments { get; set; }
        public int? RFQId { get; set; }
        public DateTime? FeedbackDate { get; set; } = new DateTime(2025, 1, 1);
        public string? Feasibilityassessment { get; set; }

        public string? DFM { get; set; }
        public string? DFT { get; set; }
        public string? StatutRFQ { get; set; }
        public string? TimeRFQ { get; set; }
        public int? MaxRevenue { get; set; }
        public int? EstimatedRevenue { get; set; }
        public int? NRE { get; set; }
        public bool IsVersion { get; set; } 
        public int VersionNumber { get; set; } 
        public int ClientId { get; set; }
        public ICollection<VersionViewModel> Versions { get; set; } = new List<VersionViewModel>();
    }
}
