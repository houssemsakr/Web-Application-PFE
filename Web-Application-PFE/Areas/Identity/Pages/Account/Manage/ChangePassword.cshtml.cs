// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

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
            ILogger<ChangePasswordModel> logger)
            ILogger<ChangePasswordModel> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public User CurrentUser { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            [Display(Name = "Mot de passe actuel")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [StringLength(100, ErrorMessage = "Le {0} doit contenir au moins {2} et au maximum {1} caractères.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            [Display(Name = "Nouveau mot de passe")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            [Display(Name = "Confirmer le nouveau mot de passe")]
            [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et la confirmation ne correspondent pas.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
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
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                        var oldFilePath = Path.Combine(_environment.WebRootPath, CurrentUser.imagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
                    var uniqueFileName = $"{CurrentUser.Id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                return RedirectToPage("./SetPassword");
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

        public async Task<IActionResult> OnPostAsync()
            // --- Gérer le changement de mot de passe s’il est demandé ---
            bool wantsToChangePassword = !string.IsNullOrWhiteSpace(Input?.OldPassword) ||
                                         !string.IsNullOrWhiteSpace(Input?.NewPassword) ||
                                         !string.IsNullOrWhiteSpace(Input?.ConfirmPassword);

            if (wantsToChangePassword)
        {
            if (!ModelState.IsValid)
                if (string.IsNullOrWhiteSpace(Input.OldPassword) || string.IsNullOrWhiteSpace(Input.NewPassword) || string.IsNullOrWhiteSpace(Input.ConfirmPassword))
            {
                    ModelState.AddModelError(string.Empty, "Tous les champs liés au mot de passe doivent être remplis.");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                if (!ModelState.IsValid)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                    return Page();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
                var changePasswordResult = await _userManager.ChangePasswordAsync(CurrentUser, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";
                await _signInManager.RefreshSignInAsync(CurrentUser);
                _logger.LogInformation("L'utilisateur a changé son mot de passe avec succès.");
            }

            StatusMessage = "Vos modifications ont été enregistrées avec succès.";
            return RedirectToPage();
        }
    }
}
