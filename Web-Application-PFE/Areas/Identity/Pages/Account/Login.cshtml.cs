using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

using System.DirectoryServices;

using System.Reflection.PortableExecutable;

using Web_Application_PFE.Models;

using Web_Application_PFE.Services;

using System.DirectoryServices.ActiveDirectory;

using Microsoft.AspNetCore.Components.Web;



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



        public bool AuthenticateUser(string username, string password)

        {

            try

            {

                using (var entry = new System.DirectoryServices.DirectoryEntry("LDAP://asteelflash.europe.lan", username, password))

                {
                   
                    if (entry.NativeObject != null)

                    {

                        using (var searcher = new DirectorySearcher(entry))

                        {

                            searcher.Filter = $"(&(ObjectClass=user)(sAMAccountName={username}))";

                            searcher.PropertiesToLoad.Add("displayName");



                            SearchResult user = searcher.FindOne();

                            if (user != null && user.Properties["displayName"].Count > 0)

                            {

                                string fullName = user.Properties["displayName"][0].ToString();

                                Console.WriteLine($"Utilisateur authentifié : {fullName}");

                                return true;

                            }

                        }

                    }

                }

            }

            catch (Exception ex)

            {

                Console.WriteLine($"Erreur lors de l'authentification : {ex.Message}");

            }



            return false;

        }







        private string NormalizeEmail(string emailInput)

        {

            if (string.IsNullOrWhiteSpace(emailInput))

                return emailInput;



            // Si l'input ne contient pas de @, on ajoute le domaine par défaut

            if (!emailInput.Contains("@"))

            {

                return $"{emailInput.Trim()}@asteelflash.com";

            }



            return emailInput.Trim();

        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null)

        {

            returnUrl ??= Url.Content("~/");

            bool testuser = false;

            string EmailActif = "";

            if (ModelState.IsValid)

            {

                //A ne pas toucher

                //testuser = AuthenticateUser(Input.Email, Input.Password);

                testuser = true;


                if (testuser == true)
                {
                    EmailActif = NormalizeEmail(Input.Email);
                    var user = await _userManager.FindByEmailAsync(EmailActif);

                    if (user != null)
                    {
                        // Connecter l'utilisateur
                        await _signInManager.SignInAsync(user, Input.RememberMe);
                        _logger.LogInformation($"User {Input.Email} logged in.");

                        // Générer le token JWT
                        var token = await _jwtService.GenerateToken(user);

                        // Ajouter le token dans les cookies
                        Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddMinutes(60)
                        });

                        // Redirection pour les requêtes AJAX
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return new JsonResult(new { token, returnUrl });
                        }

                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User account not found.");
                        return Page();
                    }
                }

            }

            return Page();

        }

    }

}