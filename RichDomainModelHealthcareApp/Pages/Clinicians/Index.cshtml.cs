using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    public class PatientsByClinicianModel : PageModel
    {
        private readonly HealthcareContext _context;
        private readonly ILogger<PatientsByClinicianModel> _logger;

        public PatientsByClinicianModel(HealthcareContext context, ILogger<PatientsByClinicianModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class ClinicianPatientsGroup
        {
            public string ClinicianName { get; set; }
            public Guid ClinicianId { get; set; }
            public int PatientCount { get; set; } // Changed from List<string> to int
        }


        public IList<ClinicianPatientsGroup> ClinicianPatientsGroups { get; set; } = new List<ClinicianPatientsGroup>();
        public IList<Clinician> AllClinicians { get; set; } = new List<Clinician>();

        public async Task OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Fetching clinician and patient data.");

                // Optimized query to fetch clinician and patient counts
                ClinicianPatientsGroups = await _context.Clinicians
                    .Select(c => new ClinicianPatientsGroup
                    {
                        ClinicianId = c.Id,
                        ClinicianName = c.Name.FirstName + " " + c.Name.LastName,
                        PatientCount = _context.Appointments.Count(a => a.ClinicianId == c.Id)
                    })
                    .ToListAsync();

                AllClinicians = await _context.Clinicians
                    .Include(c => c.Name)
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched clinician and patient data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching clinician and patient data.");
            }
        }


        public async Task<IActionResult> OnPostDeleteClinicianAsync(Guid clinicianId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete clinician with ID: {ClinicianId}", clinicianId);

                var clinician = await _context.Clinicians
                    .Include(c => c.Appointments)
                    .FirstOrDefaultAsync(c => c.Id == clinicianId);

                if (clinician != null)
                {
                    _context.Appointments.RemoveRange(clinician.Appointments);
                    _context.Clinicians.Remove(clinician);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Successfully deleted clinician with ID: {ClinicianId}", clinicianId);
                }
                else
                {
                    _logger.LogWarning("Clinician with ID: {ClinicianId} not found.", clinicianId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting clinician with ID: {ClinicianId}", clinicianId);
                return StatusCode(500, "An error occurred while deleting the clinician.");
            }

            return new JsonResult(new { success = true });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OnPostMassRemovePatientAsync(string patientName)
        {
            try
            {
                _logger.LogInformation("Attempting to mass remove patient: {PatientName}", patientName);

                var patient = await _context.Patients
                    .Include(p => p.Name)
                    .FirstOrDefaultAsync(p => (p.Name.FirstName + " " + p.Name.LastName) == patientName);

                if (patient == null)
                {
                    _logger.LogWarning("Patient {PatientName} not found.", patientName);
                    return NotFound(new { success = false, message = "Patient not found." });
                }

                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == patient.Id)
                    .ToListAsync();

                _context.Appointments.RemoveRange(appointments);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully removed patient {PatientName} from all clinicians.", patientName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while mass removing patient {PatientName}.", patientName);
                return StatusCode(500, "An error occurred while removing the patient.");
            }

            return new JsonResult(new { success = true });
        }

        public string GetPatientCountClass(int count)
        {
            if (count <= 1) return "patient-count-green";
            if (count <= 3) return "patient-count-orange";
            return "patient-count-red";
        }
    }
}
