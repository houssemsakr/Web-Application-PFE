using System.Threading.Tasks;

namespace Web_Application_PFE.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityType, string entityId, string additionalInfo = null);
    }
}