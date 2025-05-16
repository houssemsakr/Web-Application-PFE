using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Application_PFE.Controllers
{
    public class EntityClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntityClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EntityClient
        public async Task<IActionResult> Index()
        {
            var entityClients = await _context.EntityClients
                .Select(e => new EntityClientViewModel
                {
                    Id = e.Id,
                    Sales = e.Sales,
                    Customer = e.Customer,
                    EmailClient = e.EmailClient
                })
                .ToListAsync();

            return View(entityClients);
        }

        // GET: EntityClient/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateClientPartial()
        {
            return PartialView("_CreateClient", new EntityClient());
        }
        // POST: EntityClient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntityClient entityClient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entityClient);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    message = "Client créé avec succès!"
                });
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new
            {
                success = false,
                errors
            });
        }

        // GET: EntityClient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityClient = await _context.EntityClients.FindAsync(id);
            if (entityClient == null)
            {
                return NotFound();
            }

            var viewModel = new EntityClientViewModel
            {
                Id = entityClient.Id,
                Sales = entityClient.Sales,
                Customer = entityClient.Customer,
                EmailClient = entityClient.EmailClient
            };

            return View(viewModel);
        }

        // POST: EntityClient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EntityClientViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var entityClient = await _context.EntityClients.FindAsync(id);
                if (entityClient == null)
                {
                    return NotFound();
                }

                entityClient.Sales = viewModel.Sales;
                entityClient.Customer = viewModel.Customer;
                entityClient.EmailClient = viewModel.EmailClient;

                _context.Update(entityClient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: EntityClient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityClient = await _context.EntityClients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entityClient == null)
            {
                return NotFound();
            }

            var viewModel = new EntityClientViewModel
            {
                Id = entityClient.Id,
                Sales = entityClient.Sales,
                Customer = entityClient.Customer,
                EmailClient = entityClient.EmailClient
            };

            return View(viewModel);
        }

        // POST: EntityClient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entityClient = await _context.EntityClients.FindAsync(id);
            if (entityClient != null)
            {
                _context.EntityClients.Remove(entityClient);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: EntityClient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Récupérer l'entité EntityClient depuis la base de données
            var entityClient = await _context.EntityClients
                .FirstOrDefaultAsync(m => m.Id == id);

            if (entityClient == null)
            {
                return NotFound();
            }

            // Mapper EntityClient vers EntityClientViewModel
            var viewModel = new EntityClientViewModel
            {
                Id = entityClient.Id,
                Sales = entityClient.Sales,
                Customer = entityClient.Customer,
                EmailClient = entityClient.EmailClient
            };

            // Passer le ViewModel à la vue
            return View(viewModel);
        }
    }
}