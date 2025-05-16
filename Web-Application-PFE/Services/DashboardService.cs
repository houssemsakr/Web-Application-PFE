using Web_Application_PFE.Data;
using Web_Application_PFE.Services.Interfaces;
using Web_Application_PFE.ViewModels;

namespace Web_Application_PFE.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public DashboardViewModel GetDashboardStats()
        {
            return new DashboardViewModel
            {
                PendingCount = _context.AddRFQs.Count(r => r.Statut == "En attente de Validation"),
                WinCount = _context.AddRFQs.Count(r => r.StatutRFQ == "Win"),
                ValidatedCount = _context.AddRFQs.Count(r => r.Statut == "Validée"),
                RejectedCount = _context.AddRFQs.Count(r => r.Statut == "Non Validée"),
                DraftsCount = _context.Brouillons.Count()
                // Sum est calculé automatiquement dans le ViewModel
            };
        }
    }
}