using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;       
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services; 
using Web_Application_PFE.Services;
using Web_Application_PFE.Services.Interfaces;
namespace Web_Application_PFE.Controllers;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


public class AddRFQController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<AddRFQController> _logger;
    private readonly UserManager<User> _userManager;
    public AddRFQController(ApplicationDbContext context, IEmailService emailService, ILogger<AddRFQController> logger, UserManager<User> userManager)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
        _userManager = userManager;
    }    
    public IActionResult Gestion_RFQ()
    {
        return View();
    }
    // GET: AddRFQ
    public async Task<IActionResult> Index()
    {
        var addRFQs = await _context.AddRFQs.ToListAsync();
        var viewModels = addRFQs.Select(rfq => new AddRFQViewModel
        {
            Id = rfq.Id,
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
            ClientId = rfq.ClientId
        }).ToList();

        return View(viewModels);
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

    

    // GET: AddRFQ/Create
    public IActionResult Create()
    {
        try
        {
            // Récupérer la liste distincte des clients depuis la base de données
            var customers = _context.EntityClients
                .Where(c => c.Customer != null)  // Filtre pour éviter les valeurs nulles
                .Select(c => c.Customer)
                .Distinct()
                .OrderBy(c => c)  // Tri alphabétique
                .ToList();

            // Préparer les données pour la vue
            var viewModel = new AddRFQViewModel
            {
                // Initialiser les propriétés si nécessaire
                DateCreation = DateTime.Now,
                // Autres initialisations...
            };

            // Passer les données à la vue
            ViewBag.Customers = customers;

            // Optionnel: Si vous avez d'autres listes à peupler (comme les MarketSegment)
            ViewBag.MarketSegments = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Choisir..." },
        new SelectListItem { Value = "automotive", Text = "Automotive" },
        new SelectListItem { Value = "industrial", Text = "Industrial" },
        new SelectListItem { Value = "consumer", Text = "Consumer" },
        new SelectListItem { Value = "medical", Text = "Medical" }
    };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Loguer l'erreur (vous devriez avoir un système de logging)
            _logger.LogError(ex, "Erreur lors du chargement de la page Create RFQ");

            // Gérer l'erreur proprement
            TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement du formulaire.";
            return RedirectToAction("Index");  // Rediriger vers une page safe
        }
    }
    // POST: AddRFQ/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,DateCreation,Sales,Customer,QuoteName,MarketSegment,NumberOfRefToBeQuoted,MaxVolume,EstimatedVolume,SOPDate,KOdate,CustomerDatadate,MaterialLeader,MaterialDueDate,MaterialRelease,TestLeader,TestDueDate,TestRelease,VALeader,LabourDueDate,LabourRelease,CustomerDueDate,WorkingStatus,ApprovalDate,ClientId,RFQId")] AddRFQViewModel addRFQViewModel)
    {
        if (addRFQViewModel.RFQId.HasValue &&
   await _context.AddRFQs.AnyAsync(r => r.RFQId == addRFQViewModel.RFQId.Value))
        {
            ModelState.AddModelError("RFQId", "Un RFQ avec ce numéro existe déjà. Veuillez choisir un autre numéro ou créer une version.");
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            var addRFQ = new AddRFQ
            {
                CreatorId = user.Id,
                Creator = user,
                DateCreation = addRFQViewModel.DateCreation,
                Sales = addRFQViewModel.Sales,
                Customer = addRFQViewModel.Customer,
                QuoteName = addRFQViewModel.QuoteName,
                MarketSegment = addRFQViewModel.MarketSegment,
                NumberOfRefToBeQuoted = addRFQViewModel.NumberOfRefToBeQuoted,
                MaxVolume = addRFQViewModel.MaxVolume,
                EstimatedVolume = addRFQViewModel.EstimatedVolume,
                SOPDate = addRFQViewModel.SOPDate,
                KOdate = addRFQViewModel.KOdate,
                CustomerDatadate = addRFQViewModel.CustomerDatadate,
                MaterialLeader = addRFQViewModel.MaterialLeader,
                MaterialDueDate = addRFQViewModel.MaterialDueDate,
                MaterialRelease = addRFQViewModel.MaterialRelease,
                TestLeader = addRFQViewModel.TestLeader,
                TestDueDate = addRFQViewModel.TestDueDate,
                TestRelease = addRFQViewModel.TestRelease,
                VALeader = addRFQViewModel.VALeader,
                LabourDueDate = addRFQViewModel.LabourDueDate,
                LabourRelease = addRFQViewModel.LabourRelease,
                CustomerDueDate = addRFQViewModel.CustomerDueDate,
                WorkingStatus = addRFQViewModel.WorkingStatus,
                ApprovalDate = addRFQViewModel.ApprovalDate,
                ClientId = addRFQViewModel.ClientId,
                RFQId = addRFQViewModel.RFQId,
                Statut = "En attente de Validation"
            };
            _context.Add(addRFQ);
            await _context.SaveChangesAsync();
            // Envoi de l'email après l'ajout en base de données
            // Créer la notification
         
            try
            {
                string subject = "Nouvelle RFQ créée";
                string message = $"Une nouvelle RFQ a été créée avec les détails suivants :<br><br>" +
                                $"<strong>Numéro RFQ :</strong> {addRFQ.RFQId}<br>" +
                                $"<strong>Client :</strong> {addRFQ.Customer}<br>" +
                                $"<strong>QuoteName :</strong> {addRFQ.QuoteName}<br>" +
                                $"<strong>Date de création :</strong> {addRFQ.DateCreation:dd/MM/yyyy}<br><br>" +
                                "Cordialement,<br>L'équipe RFQ";

                await _emailService.SendEmailAsync("houssemsakhraoui@gmail.com", subject, message);
            }
            catch (Exception ex)
            {
                // Vous pouvez logger l'erreur si l'envoi d'email échoue
                Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
            }
            var notification = new Notification
            {
                UserName = await GetUserFullNameAsync(), // Nom de l'utilisateur connecté
                UserRole = GetUserRole(),
                CreationDate = DateTime.Now,
                RFQId = addRFQ.Id,
                CustomRFQNumber = addRFQ.RFQId,
                Comment = "",
                Status = addRFQ.Statut,
                RFQ = addRFQ
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            // Ajouter un message de succès dans TempData
            TempData["SuccessMessage"] = $"La RFQ N°{addRFQ.RFQId} a été créée avec succès. Une notification a été envoyée au validateur pour l'informer.";

            return RedirectToAction("Gestion_RFQ", "AddRFQ");
        }
        return View(addRFQViewModel);
    }

    private string GetUserRole()
    {
        if (User.IsInRole("Validateur"))
        {
            return "Validateur";
        }
        else if (User.IsInRole("Ingénieur RFQ"))
        {
            return "Ingénieur RFQ";
        }
        else
        {
            return "Utilisateur"; // Rôle par défaut si aucun des deux
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _context.Notifications
            .Include(n => n.RFQ) // Inclure les données du RFQ associé
            .OrderByDescending(n => n.CreationDate)
            .Take(10)
            .Select(n => new {
                id = n.Id,
                userName = n.UserName,
                userRole = n.UserRole,
                creationDate = n.CreationDate,
                rfqId = n.RFQId,
                comment = n.Comment,
                status = n.Status,
                isRead = n.IsRead,
                rfqnumber=n.CustomRFQNumber
            })
            .ToListAsync();

        return Json(notifications);
    }
    private async Task<string> GetUserFullNameAsync()
    {
        var userManager = HttpContext.RequestServices.GetService<UserManager<User>>();
        var user = await userManager.GetUserAsync(User);

        if (user != null && !string.IsNullOrEmpty(user.Nom) && !string.IsNullOrEmpty(user.Prenom))
        {
            return $"{user.Nom} {user.Prenom}";
        }

        return User.Identity?.Name ?? "Utilisateur inconnu";
    }
    [HttpPost]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound(new { success = false, message = "Notification not found" });
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
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
           
            return RedirectToAction("Gestion_RFQ");
        }
        var viewModel = new AddRFQViewModel
        {
           
            CreatorId = addRFQ.CreatorId,
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

    // POST: AddRFQ/Edit/5

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
           
            return RedirectToAction("Gestion_RFQ");
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
            return RedirectToAction("Gestion_RFQ", "AddRFQ");
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

    // GET: AddRFQ/Delete/5
    public async Task<IActionResult> Delete(int? id)
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
            ClientId = addRFQ.ClientId
        };

        return View(viewModel);
    }


    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            // Charger le RFQ avec ses versions associées
            var addRFQ = await _context.AddRFQs
                .Include(r => r.VersionEntities) // Important : inclure les entités liées
                .FirstOrDefaultAsync(m => m.Id == id);

            if (addRFQ == null)
            {
                return NotFound();
            }

            // 1. D'abord supprimer les versions associées
            _context.VersionEntities.RemoveRange(addRFQ.VersionEntities);

            // 2. Puis supprimer le RFQ lui-même
            _context.AddRFQs.Remove(addRFQ);

            // 3. Sauvegarder les changements
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du RFQ");

            // Retourner le message d'erreur complet pour le débogage
            return StatusCode(500, new
            {
                error = "Erreur lors de la suppression",
                detail = ex.InnerException?.Message ?? ex.Message
            });
        }
    }
    private bool AddRFQExists(int id)
    {
        return _context.AddRFQs.Any(e => e.Id == id);
    }

    // GET: AddRFQ/Create_valid
    public IActionResult Create_valid()
    {
        try
        {
            // Récupérer la liste distincte des clients depuis la base de données
            var customers = _context.EntityClients
                .Where(c => c.Customer != null)  // Filtre pour éviter les valeurs nulles
                .Select(c => c.Customer)
                .Distinct()
                .OrderBy(c => c)  // Tri alphabétique
                .ToList();

            // Préparer les données pour la vue
            var viewModel = new AddRFQViewModel
            {
                // Initialiser les propriétés si nécessaire
                DateCreation = DateTime.Now,
                // Autres initialisations...
            };

            // Passer les données à la vue
            ViewBag.Customers = customers;

            // Optionnel: Si vous avez d'autres listes à peupler (comme les MarketSegment)
            ViewBag.MarketSegments = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Choisir..." },
        new SelectListItem { Value = "automotive", Text = "Automotive" },
        new SelectListItem { Value = "industrial", Text = "Industrial" },
        new SelectListItem { Value = "consumer", Text = "Consumer" },
        new SelectListItem { Value = "medical", Text = "Medical" }
    };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Loguer l'erreur (vous devriez avoir un système de logging)
            _logger.LogError(ex, "Erreur lors du chargement de la page Create RFQ");

            // Gérer l'erreur proprement
            TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement du formulaire.";
            return RedirectToAction("Index");  // Rediriger vers une page safe
        }
    }

    // POST: AddRFQ/Create_valid
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Validateur")] // Sécurisez cette action
    public async Task<IActionResult> Create_valid([Bind("Id,DateCreation,Sales,Customer,QuoteName,MarketSegment,NumberOfRefToBeQuoted,MaxVolume,EstimatedVolume,SOPDate,KOdate,CustomerDatadate,MaterialLeader,MaterialDueDate,MaterialRelease,TestLeader,TestDueDate,TestRelease,VALeader,LabourDueDate,LabourRelease,CustomerDueDate,WorkingStatus,ApprovalDate,ClientId,RFQId")] AddRFQViewModel addRFQViewModel)
    {
        if (addRFQViewModel.RFQId.HasValue &&
    await _context.AddRFQs.AnyAsync(r => r.RFQId == addRFQViewModel.RFQId.Value))
        {
            ModelState.AddModelError("RFQId", "Un RFQ avec ce numéro existe déjà. Veuillez choisir un autre numéro ou créer une version.");
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            var addRFQ = new AddRFQ
            {
                CreatorId = user.Id,
                Creator = user,
                DateCreation = addRFQViewModel.DateCreation,
                Sales = addRFQViewModel.Sales,
                Customer = addRFQViewModel.Customer,
                QuoteName = addRFQViewModel.QuoteName,
                MarketSegment = addRFQViewModel.MarketSegment,
                NumberOfRefToBeQuoted = addRFQViewModel.NumberOfRefToBeQuoted,
                MaxVolume = addRFQViewModel.MaxVolume,
                EstimatedVolume = addRFQViewModel.EstimatedVolume,
                SOPDate = addRFQViewModel.SOPDate,
                KOdate = addRFQViewModel.KOdate,
                CustomerDatadate = addRFQViewModel.CustomerDatadate,
                MaterialLeader = addRFQViewModel.MaterialLeader,
                MaterialDueDate = addRFQViewModel.MaterialDueDate,
                MaterialRelease = addRFQViewModel.MaterialRelease,
                TestLeader = addRFQViewModel.TestLeader,
                TestDueDate = addRFQViewModel.TestDueDate,
                TestRelease = addRFQViewModel.TestRelease,
                VALeader = addRFQViewModel.VALeader,
                LabourDueDate = addRFQViewModel.LabourDueDate,
                LabourRelease = addRFQViewModel.LabourRelease,
                CustomerDueDate = addRFQViewModel.CustomerDueDate,
                WorkingStatus = addRFQViewModel.WorkingStatus,
                ApprovalDate = addRFQViewModel.ApprovalDate,
                ClientId = addRFQViewModel.ClientId,
                RFQId = addRFQViewModel.RFQId,
                Statut = "En attente de Validation"
            };
            _context.Add(addRFQ);
            await _context.SaveChangesAsync();
            // Envoi de l'email après l'ajout en base de données
            try
            {
                string subject = "Nouvelle RFQ créée";
                string message = $"Une nouvelle RFQ a été créée avec les détails suivants :<br><br>" +
                                $"<strong>Numéro RFQ :</strong> {addRFQ.RFQId}<br>" +
                                $"<strong>Client :</strong> {addRFQ.Customer}<br>" +
                                $"<strong>Nom du devis :</strong> {addRFQ.QuoteName}<br>" +
                                $"<strong>Date de création :</strong> {addRFQ.DateCreation:dd/MM/yyyy}<br><br>" +
                                "Cordialement,<br>L'équipe RFQ";

                await _emailService.SendEmailAsync("houssemsakhraoui@gmail.com", subject, message);
            }
            catch (Exception ex)
            {
                // Vous pouvez logger l'erreur si l'envoi d'email échoue
                Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
            }
            var notification = new Notification
            {
                UserName = await GetUserFullNameAsync(), // Nom de l'utilisateur connecté
                UserRole = GetUserRole(),
                CreationDate = DateTime.Now,
                RFQId = addRFQ.Id,
                CustomRFQNumber = addRFQ.RFQId,
                Comment = "",
                Status = addRFQ.Statut,
                RFQ = addRFQ
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            // Ajouter un message de succès dans TempData
            TempData["SuccessMessage"] = $"La RFQ N°{addRFQ.RFQId} a été créée avec succès. Une notification a été envoyée au validateur pour l'informer.";

            return RedirectToAction("Gestion_RFQ", "AddRFQ");
        }
        return View(addRFQViewModel);
    }
    public IActionResult PartialView1()
    {
        return PartialView("_PartialView1");
    }

    public async Task<IActionResult> PartialView2()
    {
        var addRFQs = await _context.AddRFQs.ToListAsync();
        var viewModels = addRFQs.Select(rfq => new AddRFQViewModel
        {
            Id = rfq.Id,
            CreatorId = rfq.CreatorId,
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
            ClientId = rfq.ClientId,
            RFQId = rfq.RFQId

        }).ToList();

        return PartialView("_PartialView2", viewModels);
    }
   

    public async Task<IActionResult> _Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var addRFQ = await _context.AddRFQs.FirstOrDefaultAsync(m => m.Id == id);
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
            RFQId = addRFQ.RFQId
        };

        return PartialView("_Details", viewModel);
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
    public async Task<IActionResult> CheckRFQIdExists(int? rfqId)
    {
        if (!rfqId.HasValue)
        {
            return Json(new { exists = false });
        }

        var exists = await _context.AddRFQs.AnyAsync(r => r.RFQId == rfqId.Value);
        return Json(new { exists });
    }
    // GET: AddRFQ/CreatClient
    [HttpGet]
    public IActionResult CreatClient()
    {
        var clients = _context.EntityClients.ToList();
        ViewBag.Clients = clients;
        return View(new EntityClientViewModel());
    }

    // POST: AddRFQ/CreateClient
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClient(EntityClientViewModel model)
    {
        if (ModelState.IsValid)
        {
            var client = new EntityClient
            {
                Sales = model.Sales,
                Customer = model.Customer,
                EmailClient = model.EmailClient
            };

            _context.EntityClients.Add(client);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Client créé avec succès";
            return RedirectToAction(nameof(CreatClient));
        }

        // Si le modèle n'est pas valide, rechargez la vue avec les données existantes
        var clients = _context.EntityClients.ToList();
        ViewBag.Clients = clients;
        return View("CreatClient", model);
    }

    // POST: AddRFQ/DeleteClient
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.EntityClients.FindAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        // Vérifier si le client est utilisé dans des RFQs
        var isUsed = await _context.AddRFQs.AnyAsync(r => r.ClientId == id);
        if (isUsed)
        {
            TempData["ErrorMessage"] = "Ce client ne peut pas être supprimé car il est associé à des RFQs existantes.";
            return RedirectToAction(nameof(CreatClient));
        }

        _context.EntityClients.Remove(client);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Client supprimé avec succès";
        return RedirectToAction(nameof(CreatClient));
    }
}