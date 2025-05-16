namespace Web_Application_PFE.ViewModels
{
    public class VersionViewModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public string? Sales { get; set; }
        public string? Customer { get; set; }   
        public string? QuoteName { get; set; }
        public string? MarketSegment { get; set; }
        public int NumberOfRefToBeQuoted { get; set; }
        public int MaxVolume { get; set; }
        public int EstimatedVolume { get; set; }
        public DateTime SOPDate { get; set; }
        public DateTime KOdate { get; set; }
        public DateTime CustomerDatadate { get; set; }
        public string? MaterialLeader { get; set; }
        public DateTime MaterialDueDate { get; set; }
        public DateTime MaterialRelease { get; set; }
        public string? TestLeader { get; set; }
        public DateTime TestDueDate { get; set; }
        public DateTime TestRelease { get; set; }
        public string? VALeader { get; set; }
        public DateTime LabourDueDate { get; set; }
        public DateTime LabourRelease { get; set; }
        public DateTime CustomerDueDate { get; set; }
        public string WorkingStatus { get; set; } = "Not Started";
        public DateTime ApprovalDate { get; set; }
        public DateTime? FeedbackDate { get; set; }
        public string? Comments { get; set; }
        public string? Statut { get; set; }
        public int ClientId { get; set; }
        public int AddRFQId { get; set; }
        public int? NRE { get; set; }
        public int? MaxRevenue { get; set; }
        public int? EstimatedRevenue { get; set; }
        public int? RFQId { get; set; }
        public bool IsVersion { get; set; }
        public int VersionNumber { get; set; }
        public string? Feasibilityassessment { get; set; }
        public string? DFM { get; set; }
        public string? DFT { get; set; }
        public string? StatutRFQ { get; set; }
        public string? TimeRFQ { get; set; }
        public string? CreatorFullName { get; set; }

        public string? CreatorId { get; set; }
    }
}
