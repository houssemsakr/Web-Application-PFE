using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Application_PFE.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.UI.Services;
using Web_Application_PFE.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace Web_Application_PFE.Controllers;

    public class RFQController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AddRFQController> _logger;
    private readonly UserManager<User> _userManager;
    public RFQController(ApplicationDbContext context, IEmailService emailService, ILogger<AddRFQController> logger, UserManager<User> userManager)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
        _userManager = userManager;
    }

        // Vue principale RFQ
        public IActionResult Rfq()
        {
            return View();
        }

        // Contenu RFQ
        public IActionResult GetRfqContent()
        {
            return PartialView("_RfqContent");
        }

        public IActionResult GetPartagésContent()
        {
            return PartialView("_PartagesContent");
        }

        // Contenu Brouillons - Liste tous les brouillons
        public IActionResult GetBrouillonsContent()
        {
            var brouillons = _context.Brouillons
                .Include(b => b.Client)
                .OrderByDescending(b => b.DateCreation)
                .ToList();

            return PartialView("_BrouillonsContent", brouillons);
        }
    // Modifier dans RFQController.cs
    public IActionResult GetEpinglesContent()
    {
        try
        {
            var epingles = _context.Epingles
                .Include(e => e.Client)
                .OrderByDescending(e => e.DateCreation)
                .ToList();

            // Ajoutez ce logging
            _logger.LogInformation($"Nombre d'épingles trouvés : {epingles.Count}");
            if (epingles.Any())
            {
                _logger.LogInformation($"Premier épinglé : {epingles.First().Id}");
            }

            return PartialView("_EpinglesContent", epingles ?? new List<Epingle>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des épingles");
            return PartialView("_EpinglesContent", new List<Epingle>());
        }
    }

    // GET: Édition d'un brouillon
    // GET: Édition d'un brouillon
    public IActionResult EditBrouillon(int id)
        {
            // Récupérer la liste des clients distincts
            var customers = _context.EntityClients
                .Select(c => c.Customer)
                .Distinct()
                .ToList();

            ViewBag.Customers = customers;

            var brouillon = _context.Brouillons
                .Include(b => b.Client)
                .FirstOrDefault(b => b.Id == id);

            if (brouillon == null)
            {
                return NotFound();
            }

            // Conversion du modèle en ViewModel
            var viewModel = new BrouillonViewModel
            {
                Id = brouillon.Id,
                QuoteName = brouillon.QuoteName,
                Customer = brouillon.Customer,
                Sales = brouillon.Sales,
                MarketSegment = brouillon.MarketSegment,
                NumberOfRefToBeQuoted = brouillon.NumberOfRefToBeQuoted,
                MaxVolume = brouillon.MaxVolume,
                EstimatedVolume = brouillon.EstimatedVolume,
                SOPDate = brouillon.SOPDate,
                KOdate = brouillon.KOdate,
                CustomerDatadate = brouillon.CustomerDatadate,
                MaterialLeader = brouillon.MaterialLeader,
                MaterialDueDate = brouillon.MaterialDueDate,
                TestLeader = brouillon.TestLeader,
                TestDueDate = brouillon.TestDueDate,
                VALeader = brouillon.VALeader,
                LabourDueDate = brouillon.LabourDueDate,
                CustomerDueDate = brouillon.CustomerDueDate,
                WorkingStatus = brouillon.WorkingStatus,
                Comments = brouillon.Comments,
                Statut = brouillon.Statut,
                RFQId = brouillon.RFQId,
                ClientId = brouillon.ClientId,
                DateCreation = brouillon.DateCreation
            };

            return View("Edit_Brouillon", viewModel);
        }

    // POST: Sauvegarde des modifications d'un brouillon
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConvertToRFQ(BrouillonViewModel model)
    {
        // Vérifier si le RFQId existe déjà
        if (model.RFQId.HasValue &&
            await _context.AddRFQs.AnyAsync(r => r.RFQId == model.RFQId.Value))
        {
            ModelState.AddModelError("RFQId", "Un RFQ avec ce numéro existe déjà. Veuillez choisir un autre numéro ou créer une version.");
        }

        if (ModelState.IsValid)
        {
            // Créer une nouvelle RFQ à partir du brouillon
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            var newRFQ = new AddRFQ
            {
                CreatorId = user.Id,
                Creator = user,
                DateCreation = model.DateCreation ?? DateTime.Now,
                Sales = model.Sales,
                Customer = model.Customer,
                QuoteName = model.QuoteName,
                MarketSegment = model.MarketSegment,
                NumberOfRefToBeQuoted = model.NumberOfRefToBeQuoted ?? 0,
                MaxVolume = model.MaxVolume ?? 0,
                EstimatedVolume = model.EstimatedVolume ?? 0,
                SOPDate = model.SOPDate ?? DateTime.Now,
                KOdate = model.KOdate ?? DateTime.Now,
                CustomerDatadate = model.CustomerDatadate ?? DateTime.Now,
                MaterialLeader = model.MaterialLeader,
                MaterialDueDate = model.MaterialDueDate ?? DateTime.Now,
                MaterialRelease = model.MaterialRelease ?? DateTime.Now,
                TestLeader = model.TestLeader,
                TestDueDate = model.TestDueDate ?? DateTime.Now,
                TestRelease = model.TestRelease ?? DateTime.Now,
                VALeader = model.VALeader,
                LabourDueDate = model.LabourDueDate ?? DateTime.Now,
                LabourRelease = model.LabourRelease ?? DateTime.Now,
                CustomerDueDate = model.CustomerDueDate ?? DateTime.Now,
                WorkingStatus = "Not Started",
                ApprovalDate = model.ApprovalDate ?? DateTime.Now,
                ClientId = model.ClientId,
                RFQId = model.RFQId,
                Statut = "En attente de Validation"
            };

            _context.AddRFQs.Add(newRFQ);

            // Supprimer le brouillon après conversion
            var brouillon = await _context.Brouillons.FindAsync(model.Id);
            if (brouillon != null)
            {
                _context.Brouillons.Remove(brouillon);
            }

            await _context.SaveChangesAsync();

            // Envoi de l'email de notification
            try
            {
                string subject = "Nouvelle RFQ créée à partir d'un brouillon";
                string message = $"Une nouvelle RFQ a été créée à partir d'un brouillon avec les détails suivants :<br><br>" +
                                $"<strong>Numéro RFQ :</strong> {newRFQ.RFQId}<br>" +
                                $"<strong>Client :</strong> {newRFQ.Customer}<br>" +
                                $"<strong>Nom du devis :</strong> {newRFQ.QuoteName}<br>" +
                                $"<strong>Date de création :</strong> {newRFQ.DateCreation:dd/MM/yyyy}<br><br>" +
                                "Cordialement,<br>L'équipe RFQ";

                await _emailService.SendEmailAsync("houssemsakhraoui@gmail.com", subject, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de l'envoi de l'email : {ex.Message}");
            }
            var notification = new Notification
            {
                UserName = await GetUserFullNameAsync(), // Nom de l'utilisateur connecté
                UserRole = GetUserRole(),
                CreationDate = DateTime.Now,
                RFQId = newRFQ.Id,
                CustomRFQNumber = newRFQ.RFQId,
                Comment = "",
                Status = newRFQ.Statut,
                RFQ = newRFQ
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
           
            //return RedirectToAction("Gestion_RFQ", "AddRFQ");

            TempData["SuccessMessage"] = $"Le brouillon a été converti en RFQ N°{newRFQ.RFQId} avec succès.";
            return RedirectToAction("Rfq");
        }

        // Si le modèle n'est pas valide, rechargez la vue avec les erreurs
        var customers = _context.EntityClients
            .Select(c => c.Customer)
            .Distinct()
            .ToList();
        ViewBag.Customers = customers;

        return View("Edit_Brouillon", model);
    }

    // Suppression d'un brouillon
    [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBrouillon(int id)
        {
            var brouillon = _context.Brouillons.Find(id);
            if (brouillon == null)
            {
                return NotFound();
            }

            _context.Brouillons.Remove(brouillon);
            _context.SaveChanges();

            return RedirectToAction("GetBrouillonsContent");
        }

    public async Task<IActionResult> DetailsEpingle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var epingle = await _context.Epingles
            .Include(e => e.Client)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (epingle == null)
        {
            return NotFound();
        }

        var viewModel = new EpingleViewModel
        {
            Id = epingle.Id,
            DateCreation = epingle.DateCreation,
            Sales = epingle.Sales,
            Customer = epingle.Customer,
            QuoteName = epingle.QuoteName,
            MarketSegment = epingle.MarketSegment,
            NumberOfRefToBeQuoted = epingle.NumberOfRefToBeQuoted,
            MaxVolume = epingle.MaxVolume,
            EstimatedVolume = epingle.EstimatedVolume,
            SOPDate = epingle.SOPDate,
            KOdate = epingle.KOdate,
            CustomerDatadate = epingle.CustomerDatadate,
            MaterialLeader = epingle.MaterialLeader,
            MaterialDueDate = epingle.MaterialDueDate,
            MaterialRelease = epingle.MaterialRelease,
            TestLeader = epingle.TestLeader,
            TestDueDate = epingle.TestDueDate,
            TestRelease = epingle.TestRelease,
            VALeader = epingle.VALeader,
            LabourDueDate = epingle.LabourDueDate,
            LabourRelease = epingle.LabourRelease,
            CustomerDueDate = epingle.CustomerDueDate,
            WorkingStatus = epingle.WorkingStatus,
            ApprovalDate = epingle.ApprovalDate,
            FeedbackDate = epingle.FeedbackDate,
            Comments = epingle.Comments,
            Statut = epingle.Statut,
            Feasibilityassessment = epingle.Feasibilityassessment,
            DFM = epingle.DFM,
            DFT = epingle.DFT,
            MaxRevenue = epingle.MaxRevenue,
            EstimatedRevenue = epingle.EstimatedRevenue,
            NRE = epingle.NRE,
            StatutRFQ = epingle.StatutRFQ,
            TimeRFQ = epingle.TimeRFQ,
            ClientId = epingle.ClientId
        };

        return View("Details",viewModel); 
    }
    [HttpPost]
    public async Task<IActionResult> DetachEpingle(int id)
    {
        var epingle = await _context.Epingles.FindAsync(id);
        if (epingle == null)
        {
            return NotFound();
        }

        _context.Epingles.Remove(epingle);
        await _context.SaveChangesAsync();

        // Après suppression, on recharge la liste des épingles
        var epingles = await _context.Epingles.ToListAsync();

        return PartialView("_EpinglesContent", epingles);
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

    }
