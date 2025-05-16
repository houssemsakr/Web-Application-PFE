using System.ComponentModel.DataAnnotations;

namespace Web_Application_PFE.ViewModels
{
    public class EpingleViewModel
    {
        [Key]
        public int Id { get; set; }


        public DateTime? DateCreation { get; set; } = DateTime.Now;


        [StringLength(200)]
        public string? Sales { get; set; }

        [StringLength(200)]
        public string? Customer { get; set; }

        [StringLength(200)]
        public string? QuoteName { get; set; } // Nom de la RFQ


        [StringLength(100)]
        public string? MarketSegment { get; set; } // Segment de marché


        public int? NumberOfRefToBeQuoted { get; set; }

        public int? MaxVolume { get; set; }


        public int? EstimatedVolume { get; set; }


        public DateTime? SOPDate { get; set; } // Date de début de production (SOP Date)


        public DateTime? KOdate { get; set; }


        public DateTime? CustomerDatadate { get; set; }


        public string? MaterialLeader { get; set; }


        public DateTime? MaterialDueDate { get; set; } // Date d'échéance des matériaux


        public DateTime? MaterialRelease { get; set; }


        public string? TestLeader { get; set; }

        public DateTime? TestDueDate { get; set; } // Date d'échéance des tests


        public DateTime? TestRelease { get; set; }

        public string? VALeader { get; set; }

        public DateTime? LabourDueDate { get; set; } // Date d'échéance pour la main-d'œuvre

        public DateTime? LabourRelease { get; set; }

        public DateTime? CustomerDueDate { get; set; } // Date d'échéance pour le client

        public string? WorkingStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string? Statut { get; set; }

        public string? Comments { get; set; }
        public int? RFQId { get; set; }
        public DateTime? FeedbackDate { get; set; }
        public string? Feasibilityassessment { get; set; }

        public string? DFM { get; set; }
        public string? DFT { get; set; }
        public string? StatutRFQ { get; set; }
        public string? TimeRFQ { get; set; }
        public int? MaxRevenue { get; set; }
        public int? EstimatedRevenue { get; set; }
        public int? NRE { get; set; }
        public bool IsVersion { get; set; } // Indique si c'est une version (true) ou la RFQ originale (false)
        public int VersionNumber { get; set; } // Numéro de version (0 pour RFQ originale, 1+ pour les versions)
        public int ClientId { get; set; }
        public ICollection<VersionViewModel> Versions { get; set; } = new List<VersionViewModel>();
    }
}

