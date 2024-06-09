using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly HealthcareContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(HealthcareContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Clinician Clinician { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("OnGet called - Displaying form for new clinician");

            // Setting default values for testing
            Clinician = new Clinician
            {
                Name = new Name("John", "Doe"),
                Address = new Address("123 Main St", "Anytown", "State", "12345"),
                DateOfBirth = new DateTime(1980, 1, 1),
                PhoneNumber = "123-456-7890",
                Specialty = "General Practice"
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync called - Attempting to create new clinician");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                LogModelStateErrors();
                return Page();
            }

            if (!ValidateClinician(Clinician))
            {
                _logger.LogWarning("Clinician validation failed");
                return Page();
            }

            try
            {
                _logger.LogInformation("Clinician validated successfully. Preparing to add to context");

                var clinician = new Clinician
                {
                    Name = new Name(Clinician.Name.FirstName, Clinician.Name.LastName),
                    Address = new Address(Clinician.Address.Street, Clinician.Address.City, Clinician.Address.State, Clinician.Address.ZipCode),
                    DateOfBirth = Clinician.DateOfBirth,
                    PhoneNumber = Clinician.PhoneNumber,
                    Specialty = Clinician.Specialty
                };

                _context.Clinicians.Add(clinician);
                _logger.LogInformation("Clinician added to the context, attempting to save changes");

                await _context.SaveChangesAsync();
                _logger.LogInformation("Clinician created successfully with Id: {Id}", clinician.Id);

                return RedirectToPage("/Clinicians/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clinician");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the clinician.");
                return Page();
            }
        }

        private bool ValidateClinician(Clinician clinician)
        {
            var nameRegex = new Regex(@"^[a-zA-Z]+$");
            var addressRegex = new Regex(@"^[a-zA-Z0-9\s,-]+$");

            bool isValid = true;

            if (!nameRegex.IsMatch(clinician.Name.FirstName) || !nameRegex.IsMatch(clinician.Name.LastName))
            {
                ModelState.AddModelError(string.Empty, "Names can only contain letters.");
                isValid = false;
                _logger.LogWarning("Name validation failed");
            }

            if (!addressRegex.IsMatch(clinician.Address.Street) || !addressRegex.IsMatch(clinician.Address.City) ||
                !addressRegex.IsMatch(clinician.Address.State) || !addressRegex.IsMatch(clinician.Address.ZipCode))
            {
                ModelState.AddModelError(string.Empty, "Address fields can only contain letters, numbers, spaces, commas, and hyphens.");
                isValid = false;
                _logger.LogWarning("Address validation failed");
            }

            return isValid;
        }

        private void LogModelStateErrors()
        {
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    _logger.LogWarning("Error with {Key}: {Error}", modelStateKey, error.ErrorMessage);
                }
            }
        }
    }
}
