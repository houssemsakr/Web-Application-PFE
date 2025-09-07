using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;
using Web_Application_PFE.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Web_Application_PFE.Models;
using Web_Application_PFE.Services.Interfaces;
using Microsoft.Extensions.Logging;
namespace Web_Application_PFE.Controllers
{
    public class Suivi_RFQController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<Suivi_RFQController> _logger;
        private readonly UserManager<User> _userManager;

        public Suivi_RFQController(ApplicationDbContext context, IEmailService emailService, ILogger<Suivi_RFQController> logger, UserManager<User> userManager)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
        }

        // Méthode pour afficher la liste des RFQ
        public async Task<IActionResult> SuiviRFQ()
        {
            // Récupérer les RFQ depuis la base de données
            var addRFQs = await _context.AddRFQs.ToListAsync();

            // Convertir les RFQ en ViewModel
            var viewModels = addRFQs.Select(rfq => new AddRFQViewModel
            {
                Id = rfq.Id,
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
                Statut = rfq.Statut,
                ClientId = rfq.ClientId
            }).ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ConsulterRFQ", viewModels);
            }

            return View(viewModels);
        }

        public async Task<IActionResult> View_Par_Statut()
        {
            // Récupérer toutes les RFQ depuis la base de données
            var rfqs = await _context.AddRFQs
                .Include(r => r.Client)
                .ToListAsync();

            // Convertir les RFQ en ViewModel
            var viewModels = rfqs.Select(rfq => new AddRFQViewModel
            {
                Id = rfq.Id,
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
                Statut = rfq.Statut,
                FeedbackDate = rfq.FeedbackDate,
                Comments = rfq.Comments,
                ClientId = rfq.ClientId
            }).ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ViewParStatut", viewModels);
            }

            return View(viewModels);
        }

        public IActionResult View_code_RFQ()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ViewCodeRFQ", new AddRFQViewModel());
            }

            return View(new AddRFQViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetRFQById(int rfqId)
        {
            // Récupérer la RFQ depuis la base de données
            var rfq = await _context.AddRFQs
                .Include(r => r.Client)
               .FirstOrDefaultAsync(r => r.RFQId == rfqId);

            if (rfq == null)
            {
                return NotFound();
            }

            // Convertir la RFQ en ViewModel
            var viewModel = new AddRFQViewModel
            {
                Id = rfq.Id,
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
                Statut = rfq.Statut,
                ClientId = rfq.ClientId,
                FeedbackDate = rfq.FeedbackDate,
                Comments = rfq.Comments
            };

            return Json(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addRFQ = await _context.AddRFQs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (addRFQ == null)
            {
                return NotFound();
            }

            var viewModel = new AddRFQViewModel
            {
                Id = addRFQ.Id,
                DateCreation = addRFQ.DateCreation,
                Sales = addRFQ.Sales,
                Customer = addRFQ.Customer,
                QuoteName = addRFQ.QuoteName,
                MarketSegment = addRFQ.MarketSegment,
                NumberOfRefToBeQuoted = addRFQ.NumberOfRefToBeQuoted,
                MaxVolume = addRFQ.MaxVolume,
                EstimatedVolume = addRFQ.EstimatedVolume,
                SOPDate = addRFQ.SOPDate,
                KOdate = addRFQ.KOdate,
                CustomerDatadate = addRFQ.CustomerDatadate,
                MaterialLeader = addRFQ.MaterialLeader,
                MaterialDueDate = addRFQ.MaterialDueDate,
                MaterialRelease = addRFQ.MaterialRelease,
                TestLeader = addRFQ.TestLeader,
                TestDueDate = addRFQ.TestDueDate,
                TestRelease = addRFQ.TestRelease,
                VALeader = addRFQ.VALeader,
                LabourDueDate = addRFQ.LabourDueDate,
                LabourRelease = addRFQ.LabourRelease,
                CustomerDueDate = addRFQ.CustomerDueDate,
                WorkingStatus = addRFQ.WorkingStatus,
                ApprovalDate = addRFQ.ApprovalDate,
                ClientId = addRFQ.ClientId,
                Statut = addRFQ.Statut,
                Feasibilityassessment = addRFQ.Feasibilityassessment,
                DFM = addRFQ.DFM,
                DFT = addRFQ.DFT,
                MaxRevenue = addRFQ.MaxRevenue,
                EstimatedRevenue = addRFQ.EstimatedRevenue,
                NRE = addRFQ.NRE,
                FeedbackDate = addRFQ.FeedbackDate,
                Comments = addRFQ.Comments,
                RFQId = addRFQ.RFQId,
                StatutRFQ = addRFQ.StatutRFQ,
                TimeRFQ = addRFQ.TimeRFQ,
            };

            return View(viewModel);

        }

        // GET: AddRFQ/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addRFQ = await _context.AddRFQs.FindAsync(id);
            if (addRFQ == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            var isCreator = addRFQ.CreatorId == currentUser?.Id;
            var isValidator = User.IsInRole("Validateur");

            if (!isCreator && !isValidator)
            {

                return RedirectToAction("SuiviRFQ");
            }
            var viewModel = new AddRFQViewModel
            {
                Id = addRFQ.Id,
                CreatorId = addRFQ.CreatorId,
                DateCreation = addRFQ.DateCreation,
                Sales = addRFQ.Sales,
                Customer = addRFQ.Customer,
                QuoteName = addRFQ.QuoteName,
                MarketSegment = addRFQ.MarketSegment,
                NumberOfRefToBeQuoted = addRFQ.NumberOfRefToBeQuoted,
                MaxVolume = addRFQ.MaxVolume,
                EstimatedVolume = addRFQ.EstimatedVolume,
                SOPDate = addRFQ.SOPDate,
                KOdate = addRFQ.KOdate,
                CustomerDatadate = addRFQ.CustomerDatadate,
                MaterialLeader = addRFQ.MaterialLeader,
                MaterialDueDate = addRFQ.MaterialDueDate,
                MaterialRelease = addRFQ.MaterialRelease,
                TestLeader = addRFQ.TestLeader,
                TestDueDate = addRFQ.TestDueDate,
                TestRelease = addRFQ.TestRelease,
                VALeader = addRFQ.VALeader,
                LabourDueDate = addRFQ.LabourDueDate,
                LabourRelease = addRFQ.LabourRelease,
                CustomerDueDate = addRFQ.CustomerDueDate,
                WorkingStatus = addRFQ.WorkingStatus,
                ApprovalDate = addRFQ.ApprovalDate,
                ClientId = addRFQ.ClientId,
                // Ajoutez ces propriétés spécifiques aux validateurs
                Feasibilityassessment = addRFQ.Feasibilityassessment,
                DFM = addRFQ.DFM,
                DFT = addRFQ.DFT,
                MaxRevenue = addRFQ.MaxRevenue,
                EstimatedRevenue = addRFQ.EstimatedRevenue,
                NRE = addRFQ.NRE,
                Statut = addRFQ.Statut,
                FeedbackDate = addRFQ.FeedbackDate,
                Comments = addRFQ.Comments,
                StatutRFQ = addRFQ.StatutRFQ,
                TimeRFQ = addRFQ.TimeRFQ,
                RFQId = addRFQ.RFQId,
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddRFQViewModel addRFQViewModel, string action)
        {
            if (id != addRFQViewModel.Id)
            {
                return NotFound();
            }

            var addRFQ = await _context.AddRFQs.FindAsync(id);
            if (addRFQ == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            var isCreator = addRFQ.CreatorId == currentUser?.Id;
            var isValidator = User.IsInRole("Validateur");

            if (!isCreator && !isValidator)
            {

                return RedirectToAction("SuiviRFQ");
            }
            // Mettre à jour les champs communs
            addRFQ.DateCreation = addRFQViewModel.DateCreation;
            addRFQ.Sales = addRFQViewModel.Sales;
            addRFQ.Customer = addRFQViewModel.Customer;
            addRFQ.QuoteName = addRFQViewModel.QuoteName;
            addRFQ.MarketSegment = addRFQViewModel.MarketSegment;
            addRFQ.NumberOfRefToBeQuoted = addRFQViewModel.NumberOfRefToBeQuoted;
            addRFQ.MaxVolume = addRFQViewModel.MaxVolume;
            addRFQ.EstimatedVolume = addRFQViewModel.EstimatedVolume;
            addRFQ.SOPDate = addRFQViewModel.SOPDate;
            addRFQ.KOdate = addRFQViewModel.KOdate;
            addRFQ.CustomerDatadate = addRFQViewModel.CustomerDatadate;
            addRFQ.MaterialLeader = addRFQViewModel.MaterialLeader;
            addRFQ.MaterialDueDate = addRFQViewModel.MaterialDueDate;
            addRFQ.MaterialRelease = addRFQViewModel.MaterialRelease;
            addRFQ.TestLeader = addRFQViewModel.TestLeader;
            addRFQ.TestDueDate = addRFQViewModel.TestDueDate;
            addRFQ.TestRelease = addRFQViewModel.TestRelease;
            addRFQ.VALeader = addRFQViewModel.VALeader;
            addRFQ.LabourDueDate = addRFQViewModel.LabourDueDate;
            addRFQ.LabourRelease = addRFQViewModel.LabourRelease;
            addRFQ.CustomerDueDate = addRFQViewModel.CustomerDueDate;
            addRFQ.ApprovalDate = addRFQViewModel.ApprovalDate;
            addRFQ.ClientId = addRFQViewModel.ClientId;

            // Ne mettre à jour StatutRFQ et TimeRFQ que si WorkingStatus est "Complete"
            if (addRFQ.WorkingStatus == "Complete")
            {
                addRFQ.StatutRFQ = addRFQViewModel.StatutRFQ;
                addRFQ.TimeRFQ = addRFQViewModel.TimeRFQ;
            }

            // Si c'est une validation par l'ingénieur RFQ
            if (action == "validate" && User.IsInRole("Ingénieur RFQ"))
            {
                // Correction: Vous aviez deux affectations de WorkingStatus, j'ai gardé "In Progress"
                addRFQ.WorkingStatus = "In Progress";
                addRFQ.FeedbackDate = DateTime.Now;

                TempData["SuccessMessage"] = $"La RFQ N°{addRFQ.Id} a été validée et envoyée au validateur.";
            }
            // Si c'est une modification par le validateur
            else if (User.IsInRole("Validateur"))
            {
                addRFQ.Feasibilityassessment = addRFQViewModel.Feasibilityassessment;
                addRFQ.DFM = addRFQViewModel.DFM;
                addRFQ.DFT = addRFQViewModel.DFT;
                addRFQ.MaxRevenue = addRFQViewModel.MaxRevenue;
                addRFQ.EstimatedRevenue = addRFQViewModel.EstimatedRevenue;
                addRFQ.NRE = addRFQViewModel.NRE;
                addRFQ.Statut = addRFQViewModel.Statut;
                addRFQ.FeedbackDate = addRFQViewModel.FeedbackDate;
                addRFQ.Comments = addRFQViewModel.Comments;
                addRFQ.WorkingStatus = addRFQViewModel.WorkingStatus;

                TempData["SuccessMessage"] = $"La RFQ N°{addRFQ.RFQId} a été mise à jour.";
            }

            try
            {
                _context.Update(addRFQ);
                await _context.SaveChangesAsync();
                return RedirectToAction("SuiviRFQ");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddRFQExists(addRFQ.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool AddRFQExists(int id)
        {
            return _context.AddRFQs.Any(e => e.Id == id);
        }

      
        [HttpGet]
        public async Task<IActionResult> GetClientInfoByCustomer(string customer)
        {
            if (string.IsNullOrEmpty(customer))
            {
                return NotFound();
            }

            var client = await _context.EntityClients
                .FirstOrDefaultAsync(c => c.Customer == customer);

            if (client == null)
            {
                return Json(new { sales = "", clientId = "" });
            }

            return Json(new
            {
                sales = client.Sales,
                clientId = client.Id
            });
        }
        [HttpGet]
        public async Task<IActionResult> CheckEditPermission(int id)
        {
            var addRFQ = await _context.AddRFQs.FindAsync(id);
            if (addRFQ == null)
            {
                return Json(new { hasPermission = false });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isCreator = addRFQ.CreatorId == currentUser?.Id;
            var isValidator = User.IsInRole("Validateur");

            return Json(new
            {
                hasPermission = isCreator || isValidator
            });
        }

        [HttpGet]
        public async Task<IActionResult> CheckRFQIdExists(int? rfqId)
        {
            if (!rfqId.HasValue)
            {
                return Json(new { exists = false });
            }

            var exists = await _context.AddRFQs.AnyAsync(r => r.RFQId == rfqId.Value);
            return Json(new { exists });
        }

    }
}
