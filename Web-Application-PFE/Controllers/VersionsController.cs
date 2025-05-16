using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Web_Application_PFE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Web_Application_PFE.Controllers
{
    public class VersionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<VersionsController> _logger;

        public VersionsController(ApplicationDbContext context, IEmailService emailService, UserManager<User> userManager, ILogger<VersionsController> logger)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Versions/Ajouter_une_version
        public async Task<IActionResult> Ajouter_une_version()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AjouterVersionPartial");
            }

            return View();
        }
        // GET: /Versions/Ajouter_une_version
        public IActionResult Affiche_Versions()
        {
            var versions = _context.VersionEntities
                .Include(v => v.Client)
                .Select(v => new VersionViewModel
                {
                    Id = v.Id,
                    RFQId = v.RFQId,
                    QuoteName = v.QuoteName,
                    Customer = v.Customer,
                    DateCreation = v.DateCreation,
                    Statut = v.Statut,
                    WorkingStatus = v.WorkingStatus,
                    Code = v.Code,
                    Sales = v.Sales,
                    ClientId = v.ClientId,
                    MarketSegment = v.MarketSegment,
                    NumberOfRefToBeQuoted = v.NumberOfRefToBeQuoted,
                    MaxVolume = v.MaxVolume,
                    EstimatedVolume = v.EstimatedVolume,
                    SOPDate = v.SOPDate,
                    KOdate = v.KOdate,
                    CustomerDatadate = v.CustomerDatadate,
                    MaterialLeader = v.MaterialLeader,
                    MaterialDueDate = v.MaterialDueDate,
                    MaterialRelease = v.MaterialRelease,
                    TestLeader = v.TestLeader,
                    TestDueDate = v.TestDueDate,
                    TestRelease = v.TestRelease,
                    VALeader = v.VALeader,
                    LabourDueDate = v.LabourDueDate,
                    LabourRelease = v.LabourRelease,
                    CustomerDueDate = v.CustomerDueDate,
                    ApprovalDate = v.ApprovalDate,
                    Comments = v.Comments,
                    FeedbackDate = v.FeedbackDate,
                    Feasibilityassessment = v.Feasibilityassessment,
                    DFM = v.DFM,
                    DFT = v.DFT,
                    StatutRFQ = v.StatutRFQ,
                    TimeRFQ = v.TimeRFQ,
                    MaxRevenue = v.MaxRevenue,
                    EstimatedRevenue = v.EstimatedRevenue,
                    NRE = v.NRE,
                  
                })
                .ToList();

            return View(versions);
        }
        [HttpGet]
        public async Task<IActionResult> AfficheDetails(int id, int? version)
        {
            var versionEntity = await _context.VersionEntities
                .Include(v => v.Client) // Inclusion des données du client
                .Include(v => v.AddRFQ) // Si vous avez besoin des données de la RFQ parente
                .FirstOrDefaultAsync(v => v.Id == id);

            if (versionEntity == null)
            {
                return NotFound();
            }

            var viewModel = new VersionViewModel
            {
                // Informations de base
                Id = versionEntity.Id,
                RFQId = versionEntity.RFQId,
                IsVersion = true,
                AddRFQId = versionEntity.AddRFQId,

                // Informations client
                ClientId = versionEntity.ClientId,
                Customer = versionEntity.Customer,
                Sales = versionEntity.Sales,

                // Informations RFQ
                QuoteName = versionEntity.QuoteName,
                MarketSegment = versionEntity.MarketSegment,
                NumberOfRefToBeQuoted = versionEntity.NumberOfRefToBeQuoted,
                MaxVolume = versionEntity.MaxVolume,
                EstimatedVolume = versionEntity.EstimatedVolume,

                // Dates importantes
                DateCreation = versionEntity.DateCreation,
                SOPDate = versionEntity.SOPDate,
                KOdate = versionEntity.KOdate,
                CustomerDatadate = versionEntity.CustomerDatadate,

                // Responsables
                MaterialLeader = versionEntity.MaterialLeader,
                TestLeader = versionEntity.TestLeader,
                VALeader = versionEntity.VALeader,

                // Dates matériels
                MaterialDueDate = versionEntity.MaterialDueDate,
                MaterialRelease = versionEntity.MaterialRelease,

                // Dates tests
                TestDueDate = versionEntity.TestDueDate,
                TestRelease = versionEntity.TestRelease,

                // Dates VA
                LabourDueDate = versionEntity.LabourDueDate,
                LabourRelease = versionEntity.LabourRelease,
                CustomerDueDate = versionEntity.CustomerDueDate,

                // Statuts
                WorkingStatus = versionEntity.WorkingStatus,
                Statut = versionEntity.Statut,
                ApprovalDate = versionEntity.ApprovalDate,

                // Feedback
                Comments = versionEntity.Comments,
                FeedbackDate = versionEntity.FeedbackDate,

                // Évaluations
                Feasibilityassessment = versionEntity.Feasibilityassessment,
                DFM = versionEntity.DFM,
                DFT = versionEntity.DFT,

                // Résultats RFQ
                StatutRFQ = versionEntity.StatutRFQ,
                TimeRFQ = versionEntity.TimeRFQ,

                // Financier
                MaxRevenue = versionEntity.MaxRevenue,
                EstimatedRevenue = versionEntity.EstimatedRevenue,
                NRE = versionEntity.NRE
            };
           
            return View("Affiche_Details", viewModel);
        }
        // GET: /Versions/Consulter_les_versions
        public async Task<IActionResult> Consulter_les_versions()
        {
            var rfqs = await _context.AddRFQs
                .Include(r => r.VersionEntities)
                .ToListAsync();

            var viewModel = rfqs.Select(rfq => new AddRFQViewModel
            {
                Id = rfq.Id,
                RFQId = rfq.RFQId,
                DateCreation = rfq.DateCreation,
                WorkingStatus = rfq.WorkingStatus,
                Statut = rfq.Statut,
                Versions = rfq.VersionEntities.Select(v => new VersionViewModel
                {
                    Id = v.Id,
                    RFQId = v.RFQId,
                    DateCreation = v.DateCreation,
                    WorkingStatus = v.WorkingStatus,
                    Statut = v.Statut
                }).ToList()
            }).ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ConsulterVersionsPartial", viewModel);
            }
            return RedirectToAction("Ajouter_une_version");
        }

        // GET: /Versions/GetRFQByRFQId/5
        [HttpGet]
        public async Task<IActionResult> GetRFQById(int id)
        {
            var rfq = await _context.AddRFQs
                .Include(r => r.VersionEntities)
                .FirstOrDefaultAsync(r => r.RFQId == id);

            if (rfq == null)
            {
                return NotFound(new { success = false, message = "RFQ non trouvée" });
            }

            var viewModel = new
            {
                success = true,
                id = rfq.Id, // Ajout de l'ID de la RFQ
                rfqId = rfq.RFQId,
                dateCreation = rfq.DateCreation,
                statut = rfq.Statut,
                workingStatus = rfq.WorkingStatus,
                versions = rfq.VersionEntities.Select(v => new {
                    id = v.Id,
                    rfqId = v.RFQId,
                    dateCreation = v.DateCreation,
                    workingStatus = v.WorkingStatus,
                    statut = v.Statut
                }).ToList()
            };

            return Json(viewModel);
        }

        // GET: /Versions/Ajouter_versions/5
        [HttpGet]
        [Authorize(Roles = "Validateur,Ingénieur RFQ")]
        public async Task<IActionResult> Ajouter_versions(int rfqId)
        {
            var rfq = await _context.AddRFQs
                .Include(r => r.Client)
                .Include(r => r.VersionEntities)
                .FirstOrDefaultAsync(r => r.RFQId == rfqId);

            if (rfq == null)
            {
                return NotFound();
            }

            // Vérifier si la dernière version n'est pas "Complete"
            var lastVersion = rfq.VersionEntities.OrderByDescending(v => v.DateCreation).FirstOrDefault();
            if (lastVersion != null && lastVersion.WorkingStatus != "Complete")
            {
                TempData["ErrorMessage"] = "Impossible de créer une nouvelle version : La dernière version (V" +
                                         (rfq.VersionEntities.Count) + ") n'est pas encore complète (Statut: " +
                                         lastVersion.WorkingStatus + ")";
                return RedirectToAction("Ajouter_une_version");
            }

            // Vérifier le statut de la RFQ originale
            if (rfq.WorkingStatus != "Complete")
            {
                TempData["ErrorMessage"] = "Impossible de créer une nouvelle version : La RFQ originale n'est pas complète (Statut: " +
                                         rfq.WorkingStatus + ")";
                return RedirectToAction("Ajouter_une_version");
            }

            var viewModel = new AddRFQViewModel
            {
                Id = rfq.Id,
                RFQId = rfq.RFQId,
                DateCreation = rfq.DateCreation,
                Sales = rfq.Sales,
                Customer = rfq.Customer,
                ClientId = rfq.ClientId,
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
                Comments = rfq.Comments,
                FeedbackDate = rfq.FeedbackDate,
                Feasibilityassessment = rfq.Feasibilityassessment,
                DFM = rfq.DFM,
                DFT = rfq.DFT,
                StatutRFQ = rfq.StatutRFQ,
                TimeRFQ = rfq.TimeRFQ,
                MaxRevenue = rfq.MaxRevenue,
                EstimatedRevenue = rfq.EstimatedRevenue,
                NRE = rfq.NRE
            };

            return View(viewModel);
        }

        // POST: /Versions/Ajouter_versions
        [HttpPost]
        [Authorize(Roles = "Validateur,Ingénieur RFQ")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ajouter_versions(AddRFQViewModel model)
        {
            // Supprimer la validation pour les champs optionnels
            ModelState.Remove("Feasibilityassessment");
            ModelState.Remove("DFM");
            ModelState.Remove("DFT");
            ModelState.Remove("MaxRevenue");
            ModelState.Remove("EstimatedRevenue");
            ModelState.Remove("NRE");
            ModelState.Remove("Comments");
            ModelState.Remove("FeedbackDate");
            ModelState.Remove("WorkingStatus");

            if (ModelState.IsValid)
            {
                var originalRFQ = await _context.AddRFQs
                    .FirstOrDefaultAsync(r => r.RFQId == model.RFQId);

                if (originalRFQ == null || originalRFQ.RFQId == null)
                {
                    ModelState.AddModelError("", "La RFQ originale n'a pas été trouvée ou n'a pas d'ID valide");
                    return View(model);
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user.");
                }

                var newVersion = new VersionEntity
                {
                    CreatorId = user.Id,
                    Creator = user,
                    Code = originalRFQ.RFQId.Value,
                    DateCreation = DateTime.Now,
                    Statut = "En attente de Validation", // Statut initial pour une nouvelle version
                    AddRFQId = originalRFQ.Id,
                    Sales = model.Sales,
                    Customer = model.Customer,
                    ClientId = model.ClientId,
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
                    WorkingStatus = "Not Started",
                    ApprovalDate = model.ApprovalDate,
                    Comments = model.Comments,
                    FeedbackDate = model.FeedbackDate,
                    Feasibilityassessment = model.Feasibilityassessment,
                    DFM = model.DFM,
                    DFT = model.DFT,
                    StatutRFQ = model.StatutRFQ,
                    TimeRFQ = model.TimeRFQ,
                    MaxRevenue = model.MaxRevenue,
                    EstimatedRevenue = model.EstimatedRevenue,
                    NRE = model.NRE,
                    RFQId = originalRFQ.RFQId
                };

                _context.VersionEntities.Add(newVersion);
                await _context.SaveChangesAsync();

                // Envoi de l'email pour la nouvelle version
                try
                {
                    var versionCount = await _context.VersionEntities.CountAsync(v => v.RFQId == originalRFQ.RFQId);
                    var versionNumber = versionCount; // ou versionCount + 1 selon votre logique

                    string subject = "Nouvelle version de RFQ créée";
                    string message = $"Une nouvelle version de RFQ a été créée avec les détails suivants :<br><br>" +
                                    $"<strong>Numéro RFQ original :</strong> {originalRFQ.RFQId}<br>" +
                                 $"<strong>Numéro de la version :</strong> V{versionNumber}<br>" +
                                    $"<strong>Client :</strong> {model.Customer}<br>" +
                                    $"<strong>Nom du devis :</strong> {model.QuoteName}<br>" +
                                    $"<strong>Date de création :</strong> {DateTime.Now:dd/MM/yyyy}<br><br>" +
                                    "Cordialement,<br>L'équipe RFQ";

                    await _emailService.SendEmailAsync("houssemsakhraoui@gmail.com", subject, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
                }

                //TempData["SuccessMessage"] = $"Une nouvelle version (N°{newVersion.Id}) a été créée pour la RFQ N°{originalRFQ.RFQId}. Une notification a été envoyée au validateur.";
                return RedirectToAction("Ajouter_une_version");
            }

            return View(model);
        }
        // GET: /Versions/Ajouter_versions/5

        //private bool VersionExists(int id)
        //{
        //    return _context.VersionEntities.Any(e => e.Id == id);
        //}
        [HttpGet]
        [Authorize(Roles = "Validateur,Ingénieur RFQ")]
        public async Task<IActionResult> View_Version(int id, int? version)
        {
            // Si c'est une version (V1, V2, etc.)
            if (version.HasValue && version > 0)
            {
                var versionEntity = await _context.VersionEntities
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (versionEntity == null)
                {
                    return NotFound();
                }
                var currentUser = await _userManager.GetUserAsync(User);
                var isCreator = versionEntity.CreatorId == currentUser?.Id;
                var isValidator = User.IsInRole("Validateur");

                if (!isCreator && !isValidator)
                {

                    return RedirectToAction("Ajouter_une_version");
                }
                var viewModel = new VersionViewModel
                {
                    CreatorId = versionEntity.CreatorId,
                    Id = versionEntity.Id,
                    Code = versionEntity.Code,
                    DateCreation = versionEntity.DateCreation,
                    Sales = versionEntity.Sales,
                    Customer = versionEntity.Customer,
                    ClientId = versionEntity.ClientId,
                    QuoteName = versionEntity.QuoteName,
                    MarketSegment = versionEntity.MarketSegment,
                    NumberOfRefToBeQuoted = versionEntity.NumberOfRefToBeQuoted,
                    MaxVolume = versionEntity.MaxVolume,
                    EstimatedVolume = versionEntity.EstimatedVolume,
                    SOPDate = versionEntity.SOPDate,
                    KOdate = versionEntity.KOdate,
                    CustomerDatadate = versionEntity.CustomerDatadate,
                    MaterialLeader = versionEntity.MaterialLeader,
                    MaterialDueDate = versionEntity.MaterialDueDate,
                    MaterialRelease = versionEntity.MaterialRelease,
                    TestLeader = versionEntity.TestLeader,
                    TestDueDate = versionEntity.TestDueDate,
                    TestRelease = versionEntity.TestRelease,
                    VALeader = versionEntity.VALeader,
                    LabourDueDate = versionEntity.LabourDueDate,
                    LabourRelease = versionEntity.LabourRelease,
                    CustomerDueDate = versionEntity.CustomerDueDate,
                    WorkingStatus = versionEntity.WorkingStatus,
                    ApprovalDate = versionEntity.ApprovalDate,
                    Statut = versionEntity.Statut,
                    Comments = versionEntity.Comments,
                    FeedbackDate = versionEntity.FeedbackDate,
                    Feasibilityassessment = versionEntity.Feasibilityassessment,
                    DFM = versionEntity.DFM,
                    DFT = versionEntity.DFT,
                    StatutRFQ = versionEntity.StatutRFQ,
                    TimeRFQ = versionEntity.TimeRFQ,
                    MaxRevenue = versionEntity.MaxRevenue,
                    EstimatedRevenue = versionEntity.EstimatedRevenue,
                    NRE = versionEntity.NRE,
                    RFQId = versionEntity.RFQId,
                    IsVersion = true,
                    VersionNumber = version.Value,
                    AddRFQId = versionEntity.AddRFQId
                };

                return View(viewModel);
            }
            // Sinon, c'est la RFQ originale (V0)
            else
            {
                var rfq = await _context.AddRFQs
                    .Include(r => r.Client)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rfq == null)
                {
                    return NotFound();
                }

                var viewModel = new VersionViewModel
                {
                    Id = rfq.Id,
                    Code = rfq.RFQId ?? 0,
                    DateCreation = rfq.DateCreation,
                    Sales = rfq.Sales,
                    Customer = rfq.Customer,
                    ClientId = rfq.ClientId,
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
                    Comments = rfq.Comments,
                    FeedbackDate = rfq.FeedbackDate,
                    Feasibilityassessment = rfq.Feasibilityassessment,
                    DFM = rfq.DFM,
                    DFT = rfq.DFT,
                    StatutRFQ = rfq.StatutRFQ,
                    TimeRFQ = rfq.TimeRFQ,
                    MaxRevenue = rfq.MaxRevenue,
                    EstimatedRevenue = rfq.EstimatedRevenue,
                    NRE = rfq.NRE,
                    RFQId = rfq.RFQId,
                    IsVersion = false,
                    VersionNumber = 0,
                    AddRFQId = rfq.Id
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Validateur,Ingénieur RFQ")] // Exclut Lecteur
        public async Task<IActionResult> View_Version(VersionViewModel model)
        {
            // Vérifier que c'est bien une version (V1, V2...)
            if (!model.IsVersion)
            {
                TempData["ErrorMessage"] = "Seules les versions peuvent être modifiées";
                return RedirectToAction("Ajouter_une_version");
            }

            // Supprimer la validation pour les champs optionnels
            ModelState.Remove("Feasibilityassessment");
            ModelState.Remove("DFM");
            ModelState.Remove("DFT");
            ModelState.Remove("MaxRevenue");
            ModelState.Remove("EstimatedRevenue");
            ModelState.Remove("NRE");
            ModelState.Remove("Comments");
            ModelState.Remove("FeedbackDate");

            if (ModelState.IsValid)
            {
                var versionToUpdate = await _context.VersionEntities
                    .FirstOrDefaultAsync(v => v.Id == model.Id);

                if (versionToUpdate == null)
                {
                    return NotFound();
                }
                var currentUser = await _userManager.GetUserAsync(User);
                var isCreator = versionToUpdate.CreatorId == currentUser?.Id;
                var isValidator = User.IsInRole("Validateur");

                if (!isCreator && !isValidator)
                {

                    return RedirectToAction("Ajouter_une_version");
                }
                // Mise à jour des propriétés
                versionToUpdate.Sales = model.Sales;
                versionToUpdate.Customer = model.Customer;
                versionToUpdate.ClientId = model.ClientId;
                versionToUpdate.QuoteName = model.QuoteName;
                versionToUpdate.MarketSegment = model.MarketSegment;
                versionToUpdate.NumberOfRefToBeQuoted = model.NumberOfRefToBeQuoted;
                versionToUpdate.MaxVolume = model.MaxVolume;
                versionToUpdate.EstimatedVolume = model.EstimatedVolume;
                versionToUpdate.SOPDate = model.SOPDate;
                versionToUpdate.KOdate = model.KOdate;
                versionToUpdate.CustomerDatadate = model.CustomerDatadate;
                versionToUpdate.MaterialLeader = model.MaterialLeader;
                versionToUpdate.MaterialDueDate = model.MaterialDueDate;
                versionToUpdate.MaterialRelease = model.MaterialRelease;
                versionToUpdate.TestLeader = model.TestLeader;
                versionToUpdate.TestDueDate = model.TestDueDate;
                versionToUpdate.TestRelease = model.TestRelease;
                versionToUpdate.VALeader = model.VALeader;
                versionToUpdate.LabourDueDate = model.LabourDueDate;
                versionToUpdate.LabourRelease = model.LabourRelease;
                versionToUpdate.CustomerDueDate = model.CustomerDueDate;
                versionToUpdate.WorkingStatus = model.WorkingStatus;
                versionToUpdate.ApprovalDate = model.ApprovalDate;
                versionToUpdate.Statut = model.Statut;
                versionToUpdate.Comments = model.Comments;
                versionToUpdate.FeedbackDate = model.FeedbackDate;
                versionToUpdate.Feasibilityassessment = model.Feasibilityassessment;
                versionToUpdate.DFM = model.DFM;
                versionToUpdate.DFT = model.DFT;
                versionToUpdate.StatutRFQ = model.StatutRFQ;
                versionToUpdate.TimeRFQ = model.TimeRFQ;
                versionToUpdate.MaxRevenue = model.MaxRevenue;
                versionToUpdate.EstimatedRevenue = model.EstimatedRevenue;
                versionToUpdate.NRE = model.NRE;

                try
                {
                    _context.Update(versionToUpdate);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"La version V{model.VersionNumber} a été mise à jour avec succès";
                    return RedirectToAction("Ajouter_une_version");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!_context.VersionEntities.Any(e => e.Id == model.Id))
                    {
                        return NotFound();
                    }
                    TempData["ErrorMessage"] = "Erreur de concurrence lors de la mise à jour : " + ex.Message;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Une erreur est survenue : " + ex.Message;
                }
            }

            // Si le modèle n'est pas valide, réafficher le formulaire
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVersion(int id)
        {
            try
            {
                var version = await _context.VersionEntities.FindAsync(id);
                if (version == null)
                {
                    return Json(new { success = false, message = "Version non trouvée" });
                }

                _context.VersionEntities.Remove(version);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Version supprimée avec succès"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Erreur lors de la suppression: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetClientInfo(int clientId)
        {
            var client = await _context.EntityClients.FindAsync(clientId);
            if (client == null)
            {
                return NotFound();
            }

            return Json(new { sales = client.Sales, customer = client.Customer });
        }

    }
}