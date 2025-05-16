using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Web_Application_PFE.Models;
using Web_Application_PFE.Services;
//using System.DirectoryServices;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Reflection.PortableExecutable;

namespace Web_Application_PFE.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly JwtService _jwtService;

        public LoginModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<LoginModel> logger,
            JwtService jwtService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _jwtService = jwtService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            public string AuthType { get; set; } // "DB" ou "LDAP"
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        private bool AuthenticateUser(string username, string password, out string fullName)
        {
            fullName = null;

            //try
            //{
            //    using (DirectoryEntry entry = new DirectoryEntry("LDAP://asteelflash.europe.lan", username, password))
            //    {
            //        object nativeObject = entry.NativeObject;

            //        DirectorySearcher searcher = new DirectorySearcher(entry)
            //        {
            //            Filter = $"(&(ObjectClass=user)(sAMAccountName={username}))"
            //        };

            //        SearchResult user = searcher.FindOne();
            //        if (user != null)
            //        {
            //            fullName = user.Properties["displayName"][0]?.ToString();
            //            return true;
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                if (Input.AuthType == "LDAP")
                {
                    // Authentification LDAP
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }

                    if (AuthenticateUser(Input.Email, Input.Password, out string fullName))
                    {
                        await _signInManager.SignInAsync(user, Input.RememberMe);
                        _logger.LogInformation($"User {Input.Email} logged in via LDAP.");

                        var token = await _jwtService.GenerateToken(user);
                        Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddMinutes(60)
                        });

                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return new JsonResult(new { token, returnUrl });
                        }

                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid LDAP credentials.");
                        return Page();
                    }
                }
                else
                {
                    // Authentification normale (base de données)
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");

                        var user = await _userManager.FindByEmailAsync(Input.Email);
                        var token = await _jwtService.GenerateToken(user);

                        Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddMinutes(60)
                        });

                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return new JsonResult(new { token, returnUrl });
                        }

                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
            }

            return Page();
        }
    }
}