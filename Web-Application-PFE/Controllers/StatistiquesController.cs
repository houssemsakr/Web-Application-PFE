using Microsoft.AspNetCore.Mvc;
using Web_Application_PFE.Data;
using System.Linq;

namespace Web_Application_PFE.Controllers
{
    public class StatistiquesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatistiquesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Statistiques()
        {
            // Récupérer les statuts des RFQ
            var rfqStatuts = _context.AddRFQs
                .GroupBy(r => r.WorkingStatus)
                .Select(g => new
                {
                    WorkingStatus = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Calculer les pourcentages pour le premier graphique
            var total = rfqStatuts.Sum(s => s.Count);
            var statutsPourcentages = rfqStatuts.Select(s => new
            {
                Statut = s.WorkingStatus,
                Pourcentage = total > 0 ? (s.Count * 100.0 / total) : 0
            }).ToList();

            // Récupérer les StatutRFQ (Win/Loss) uniquement pour les RFQ validées
            var rfqWinLoss = _context.AddRFQs
                .Where(r => r.WorkingStatus == "Complete" && (r.StatutRFQ == "Win" || r.StatutRFQ == "Loss"))
                .GroupBy(r => r.StatutRFQ)
                .Select(g => new
                {
                    StatutRFQ = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Calculer les pourcentages pour le deuxième graphique
            var totalValid = rfqWinLoss.Sum(s => s.Count);
            var winLossPourcentages = rfqWinLoss.Select(s => new
            {
                StatutRFQ = s.StatutRFQ,
                Pourcentage = totalValid > 0 ? (s.Count * 100.0 / totalValid) : 0
            }).ToList();

            // Passer les données à la vue
            ViewBag.StatutsPourcentages = statutsPourcentages;
            ViewBag.WinLossPourcentages = winLossPourcentages;

            return View();
        }
    }
}