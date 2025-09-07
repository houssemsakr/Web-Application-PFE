using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.Services.Interfaces;
using Web_Application_PFE.ViewModels;

namespace Web_Application_PFE.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuditService _auditService;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager, IAuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Nom = user.Nom,
                    Prenom = user.Prenom,
                    Email = user.Email,
                    Role = role
                });
            }

            return View(userViewModels);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                Role = role,
                ExistingImagePath = user.imagePath
            };

            return View(userViewModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
            };

            return View(userViewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    if (userViewModel.ProfileImage != null && userViewModel.ProfileImage.Length > 0)
                    {
                        var uploadsFolder = Path.Combine("wwwroot", "images", "profiles");
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(userViewModel.ProfileImage.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await userViewModel.ProfileImage.CopyToAsync(fileStream);
                        }

                        if (!string.IsNullOrEmpty(user.imagePath))
                        {
                            var oldImagePath = Path.Combine("wwwroot", user.imagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        user.imagePath = "/images/profiles/" + uniqueFileName;
                    }

                    // Update user properties
                    user.Nom = userViewModel.Nom;
                    user.Prenom = userViewModel.Prenom;
                    user.Email = userViewModel.Email;
                    user.UserName = userViewModel.Email;

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Gestion du Role
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles);

                    if (!string.IsNullOrEmpty(userViewModel.Role))
                    {
                        await _userManager.AddToRoleAsync(user, userViewModel.Role);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userViewModel);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
            };

            return View(userViewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

       
    }
}
