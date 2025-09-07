using System;
using System.Threading.Tasks;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Web_Application_PFE.Services.Interfaces;

namespace Web_Application_PFE.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public AuditService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task LogAsync(string action, string entityType, string entityId, string additionalInfo = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var userId = user?.Id;
                var userName = user?.UserName;
                var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

                // Simplifier les logs pour les actions importantes
                var logAction = action;
                if (entityId == "/Identity/Account/Logout")
                {
                    logAction = "Déconnexion";
                }
                else if (entityId == "/Versions/DeleteVersion")
                {
                    logAction = "Suppression Version";
                }

                var log = new AuditLog
                {
                    UserId = userId,
                    User = user, // Assurez-vous que votre modèle AuditLog a une propriété User
                    Action = logAction,
                    EntityType = entityType,
                    EntityId = entityId,
                    Timestamp = DateTime.UtcNow,
                    IpAddress = ip,
                    AdditionalInfo = additionalInfo ?? $"{userName} a effectué cette action"
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la journalisation: {ex.Message}");
            }
        }
    }
}