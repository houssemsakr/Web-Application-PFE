using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.Services;
using Web_Application_PFE.ViewModels;

namespace Web_Application_PFE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Add DbContext

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                PinnedCount = _context.Epingles.Count(),
                WinCount = _context.AddRFQs.Count(r => r.StatutRFQ == "Win"),


                ValidatedCount = _context.AddRFQs.Count(r => r.Statut == "Validée"),
                RejectedCount = _context.AddRFQs.Count(r => r.Statut == "Non Validée"),
                PendingCount = _context.AddRFQs.Count(r => r.Statut == "En attente de Validation"),
                DraftsCount = _context.Brouillons.Count()

            };

            return View(model);
        }







        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult RFQ()
        {
            ViewData["PageTitle"] = "RFQ";
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }


    }
}