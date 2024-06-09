using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace RichDomainModelHealthcareApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public bool RememberMe { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            _logger.LogInformation("Navigated to login page. ReturnUrl: {ReturnUrl}", ReturnUrl);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            _logger.LogInformation("Login attempt with email: {Email}", Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for email: {Email}. Errors: {Errors}", Email, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully.", Email);
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning("User {Email} requires two-factor authentication.", Email);
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} account locked out.", Email);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid login attempt for user {Email}.", Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}
