using Microsoft.AspNetCore.Mvc;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web_Application_PFE.Controllers
{
    public class BrouillonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrouillonController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnregistrerBrouillon(AddRFQViewModel model)
        {
            // Vérification obligatoire du champ Customer
            if (string.IsNullOrEmpty(model.Customer))
            {
                ModelState.AddModelError("Customer", "Le champ Customer est obligatoire.");
                return View("Create", model);
            }

            // Création d'un nouveau ModelState sans les erreurs (sauf pour Customer)
            var modelStateWithoutErrors = new ModelStateDictionary();

            // Copier seulement l'erreur Customer si elle existe
            if (ModelState["Customer"]?.Errors.Count > 0)
            {
                foreach (var error in ModelState["Customer"].Errors)
                {
                    modelStateWithoutErrors.AddModelError("Customer", error.ErrorMessage);
                }
            }

            try
            {
                // Mapper vers le modèle Brouillon
                var brouillon = new Brouillon
                {
                    DateCreation = DateTime.Now,
                    Customer = model.Customer,
                    Sales = model.Sales,
                    QuoteName = model.QuoteName,
                    MarketSegment = model.MarketSegment,
                    NumberOfRefToBeQuoted = model.NumberOfRefToBeQuoted,
                    MaxVolume = model.MaxVolume,
                    EstimatedVolume = model.EstimatedVolume,
                    SOPDate = model.SOPDate,
                    KOdate = model.KOdate,
                    CustomerDatadate = model.CustomerDatadate,
                    MaterialLeader = model.MaterialLeader,
                    MaterialDueDate = model.MaterialDueDate,
                    MaterialRelease = model.MaterialRelease,
                    TestLeader = model.TestLeader,
                    TestDueDate = model.TestDueDate,
                    TestRelease = model.TestRelease,
                    VALeader = model.VALeader,
                    LabourDueDate = model.LabourDueDate,
                    LabourRelease = model.LabourRelease,
                    CustomerDueDate = model.CustomerDueDate,
                    WorkingStatus = model.WorkingStatus,
                    ApprovalDate = model.ApprovalDate,
                    FeedbackDate = model.FeedbackDate,
                    Comments = model.Comments,
                    Statut = "Brouillon",
                    Feasibilityassessment = model.Feasibilityassessment,
                    DFM = model.DFM,
                    DFT = model.DFT,
                    MaxRevenue = model.MaxRevenue,
                    EstimatedRevenue = model.EstimatedRevenue,
                    NRE = model.NRE,
                    StatutRFQ = model.StatutRFQ,
                    TimeRFQ = model.TimeRFQ,
                    RFQId = model.RFQId,
                    ClientId = model.ClientId,
                   
                };

                _context.Brouillons.Add(brouillon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Le brouillon a été enregistré avec succès.";
                return RedirectToAction("Gestion_RFQ", "AddRFQ");
            }
            catch (Exception ex)
            {
                // Log l'erreur
                // _logger.LogError(ex, "Erreur lors de l'enregistrement du brouillon");

                // Restaurer seulement l'erreur Customer
                ModelState.Clear();
                if (modelStateWithoutErrors["Customer"]?.Errors.Count > 0)
                {
                    foreach (var error in modelStateWithoutErrors["Customer"].Errors)
                    {
                        ModelState.AddModelError("Customer", error.ErrorMessage);
                    }
                }

                ModelState.AddModelError("", "Une erreur est survenue lors de l'enregistrement du brouillon.");
                return View("Create", model);
            }
        }
    }
}