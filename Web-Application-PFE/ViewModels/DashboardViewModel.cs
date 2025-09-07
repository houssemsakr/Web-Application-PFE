using System.ComponentModel.DataAnnotations;
using Web_Application_PFE.Models;
using System;
namespace Web_Application_PFE.ViewModels
{
    public class DashboardViewModel
    {
        public int PinnedCount { get; set; }
        public int WinCount { get; set; }
        public int ValidatedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingCount { get; set; }
        public int DraftsCount { get; set; }
        public int Sum => PinnedCount + WinCount + ValidatedCount + RejectedCount + PendingCount + DraftsCount;
    }
}
