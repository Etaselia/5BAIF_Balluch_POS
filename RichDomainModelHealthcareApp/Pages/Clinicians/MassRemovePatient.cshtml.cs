using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    [Authorize(Roles = "Admin")]
    public class MassRemovePatientModel : PageModel
    {
        private readonly HealthcareContext _context;
        private readonly ILogger<MassRemovePatientModel> _logger;

        public MassRemovePatientModel(HealthcareContext context, ILogger<MassRemovePatientModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Guid SelectedPatientId { get; set; }

        public SelectList PatientsSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Fetching patients for dropdown list.");

            var patients = await _context.Patients
                .Include(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    FullName = p.Name.FirstName + " " + p.Name.LastName
                })
                .ToListAsync();

            PatientsSelectList = new SelectList(patients, "Id", "FullName");

            _logger.LogInformation("Successfully fetched patients for dropdown list.");

            return Page();
        }

        public async Task<IActionResult> OnGetClinicianCountForPatientAsync(Guid patientId)
        {
            var clinicianCount = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Select(a => a.ClinicianId)
                .Distinct()
                .CountAsync();

            return new JsonResult(new { clinicianCount });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to mass remove patient with ID: {SelectedPatientId}", SelectedPatientId);

                var patient = await _context.Patients
                    .Include(p => p.Name)
                    .FirstOrDefaultAsync(p => p.Id == SelectedPatientId);

                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {SelectedPatientId} not found.", SelectedPatientId);
                    return NotFound(new { success = false, message = "Patient not found." });
                }

                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == SelectedPatientId)
                    .ToListAsync();

                _context.Appointments.RemoveRange(appointments);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully removed patient {PatientName} from all clinicians.", patient.Name.FirstName + " " + patient.Name.LastName);

                return RedirectToPage("/Clinicians/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while mass removing patient with ID {SelectedPatientId}.", SelectedPatientId);
                return StatusCode(500, "An error occurred while removing the patient.");
            }
        }
    }
}
