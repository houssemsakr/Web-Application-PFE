using Microsoft.AspNetCore.Mvc;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Web_Application_PFE.Controllers
{
    public class Gestion_RFQController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Gestion_RFQController> _logger;

        public Gestion_RFQController(ApplicationDbContext context, ILogger<Gestion_RFQController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Gestion_RFQ()
        {
            return View();
        }

        public IActionResult Créer_RFQ()
        {
            var rfqs = _context.AddRFQs.ToList(); // Récupérer toutes les RFQ depuis la base de données
            return View(rfqs); // Passer la liste des RFQ à la vue
        }
       
        public IActionResult ViewRFQ(int id)
{
    // Récupérer le RFQ correspondant à l'ID
    var rfq = _context.AddRFQs.FirstOrDefault(r => r.Id == id);

    if (rfq == null)
    {
        return NotFound(); // Retourne une erreur 404 si le RFQ n'est pas trouvé
    }

    // Passer le RFQ à la vue
    return View(rfq);
}
        public IActionResult AjoutRFQ()
        {
            return View();
        }

        public IActionResult Ajout2RFQ()
        {
            return View();
        }
      
        public IActionResult EditRFQ(int id)
        {
            // Récupérer le RFQ correspondant à l'ID
            var rfq = _context.AddRFQs.FirstOrDefault(r => r.Id == id);

            if (rfq == null)
            {
                return NotFound(); // Retourne une erreur 404 si le RFQ n'est pas trouvé
            }

            // Passer le RFQ à la vue
            return View(rfq);
        }

        [HttpPost]
        public IActionResult EditRFQ(AddRFQ rfq)
        {
            if (ModelState.IsValid)
            {
                _context.Update(rfq);
                _context.SaveChanges();
                return RedirectToAction("Créer_RFQ"); // Rediriger vers la liste des RFQ après la modification
            }

            // Si le modèle n'est pas valide, retourner à la vue avec les erreurs
            return View(rfq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PinRFQ(int rfqId)
        {
            try
            {
                // 1. Récupération du RFQ
                var rfq = await _context.AddRFQs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RFQId == rfqId);

                if (rfq == null)
                {
                    return StatusCode(404, new { error = "RFQ non trouvé" });
                }

                // 2. Création de l'Epingle avec mapping manuel
                var epingle = new Epingle
                {
                    RFQId = rfq.RFQId,
                    DateCreation = rfq.DateCreation,
                    Sales = rfq.Sales,
                    Customer = rfq.Customer,
                    QuoteName = rfq.QuoteName,
                    MarketSegment = rfq.MarketSegment,
                    NumberOfRefToBeQuoted = rfq.NumberOfRefToBeQuoted,
                    MaxVolume = rfq.MaxVolume,
                    EstimatedVolume = rfq.EstimatedVolume,
                    SOPDate = rfq.SOPDate,
                    KOdate = rfq.KOdate,
                    CustomerDatadate = rfq.CustomerDatadate,
                    MaterialLeader = rfq.MaterialLeader,
                    MaterialDueDate = rfq.MaterialDueDate,
                    MaterialRelease = rfq.MaterialRelease,
                    TestLeader = rfq.TestLeader,
                    TestDueDate = rfq.TestDueDate,
                    TestRelease = rfq.TestRelease,
                    VALeader = rfq.VALeader,
                    LabourDueDate = rfq.LabourDueDate,
                    LabourRelease = rfq.LabourRelease,
                    CustomerDueDate = rfq.CustomerDueDate,
                    WorkingStatus = rfq.WorkingStatus,
                    ApprovalDate = rfq.ApprovalDate,
                    FeedbackDate = rfq.FeedbackDate,
                    Comments = rfq.Comments,
                    Statut = rfq.Statut,
                    Feasibilityassessment = rfq.Feasibilityassessment,
                    DFM = rfq.DFM,
                    DFT = rfq.DFT,
                    MaxRevenue = rfq.MaxRevenue,
                    EstimatedRevenue = rfq.EstimatedRevenue,
                    NRE = rfq.NRE,
                    StatutRFQ = rfq.StatutRFQ,
                    TimeRFQ = rfq.TimeRFQ,
                    ClientId = rfq.ClientId
                };

                // 3. Sauvegarde
                _context.Epingles.Add(epingle);
                await _context.SaveChangesAsync();

                // 4. Réponse simplifiée
                return Ok(new
                {
                    status = "success",
                    message = $"RFQ {rfq.RFQId} épinglé"
                });
            }
            catch (Exception ex)
            {
                // Journalisation critique
                _logger.LogError(ex, "Erreur critique dans PinRFQ");

                return StatusCode(500, new
                {
                    status = "error",
                    error = "Erreur technique",
                    details = ex.GetBaseException().Message
                });
            }
        }
    }
}