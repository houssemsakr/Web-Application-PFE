using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;

namespace Web_Application_PFE.Controllers
{
    [Authorize(Roles = "Admin,Validateur")]
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ajoutez la pagination pour gérer les nombreux logs
        public async Task<IActionResult> Journal(int page = 1, int pageSize = 10)
        {
            // Filtrez les actions que vous souhaitez afficher
            var allowedActions = new List<string>
    {
        "Authentification",
        "/Identity/Account/Logout",
        "/Versions/DeleteVersion",
        "/" // Page d'accueil après authentification
    };

            var query = _context.AuditLogs
                .Where(log =>
                    log.Action == "Authentification" ||
                    log.EntityId == "/Identity/Account/Logout" ||
                    log.EntityId == "/Versions/DeleteVersion" ||
                    log.EntityId == "/")
                .OrderByDescending(log => log.Timestamp);

            var totalItems = await query.CountAsync();
            var logs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(logs);
        }
    }
}
