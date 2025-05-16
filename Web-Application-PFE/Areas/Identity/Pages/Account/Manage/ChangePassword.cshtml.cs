using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Web_Application_PFE.Models;

namespace Web_Application_PFE.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public ChangePasswordModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ChangePasswordModel> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _environment = environment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public User CurrentUser { get; set; }

        public class InputModel
        {
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe actuel")]
            public string OldPassword { get; set; }

            [StringLength(100, ErrorMessage = "Le {0} doit contenir au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nouveau mot de passe")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmer le nouveau mot de passe")]
            [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec l'ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec l'ID '{_userManager.GetUserId(User)}'.");
            }

            // --- Gérer la photo de profil ---
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                try
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(ProfilePicture.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(string.Empty, "Seuls les fichiers JPG, JPEG et PNG sont autorisés");
                        return Page();
                    }

                    if (ProfilePicture.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "La photo ne doit pas dépasser 5MB");
                        return Page();
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "profile-pictures");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    if (!string.IsNullOrEmpty(CurrentUser.imagePath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, CurrentUser.imagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uniqueFileName = $"{CurrentUser.Id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfilePicture.CopyToAsync(fileStream);
                    }

                    CurrentUser.imagePath = $"/profile-pictures/{uniqueFileName}";
                    var updateResult = await _userManager.UpdateAsync(CurrentUser);

                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'enregistrement de la photo de profil");
                    ModelState.AddModelError(string.Empty, "Une erreur s'est produite lors de l'enregistrement de la photo.");
                    return Page();
                }
            }

            // --- Gérer le changement de mot de passe s’il est demandé ---
            bool wantsToChangePassword = !string.IsNullOrWhiteSpace(Input?.OldPassword) ||
                                         !string.IsNullOrWhiteSpace(Input?.NewPassword) ||
                                         !string.IsNullOrWhiteSpace(Input?.ConfirmPassword);

            if (wantsToChangePassword)
            {
                if (string.IsNullOrWhiteSpace(Input.OldPassword) || string.IsNullOrWhiteSpace(Input.NewPassword) || string.IsNullOrWhiteSpace(Input.ConfirmPassword))
                {
                    ModelState.AddModelError(string.Empty, "Tous les champs liés au mot de passe doivent être remplis.");
                    return Page();
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(CurrentUser, Input.OldPassword, Input.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(CurrentUser);
                _logger.LogInformation("L'utilisateur a changé son mot de passe avec succès.");
            }

            StatusMessage = "Vos modifications ont été enregistrées avec succès.";
            return RedirectToPage();
        }
    }
}
