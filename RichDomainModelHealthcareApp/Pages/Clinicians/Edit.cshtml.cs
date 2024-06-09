using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly HealthcareContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(HealthcareContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Clinician Clinician { get; set; }

        [BindProperty]
        public Guid? SelectedPatientId { get; set; }

        [BindProperty]
        public string Reason { get; set; }

        public SelectList PatientsSelectList { get; set; }
        public IList<PatientAppointmentViewModel> AssignedPatients { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            _logger.LogInformation("OnGetAsync called with id: {Id}", id);

            Clinician = await _context.Clinicians
                .Include(c => c.Name)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Clinician == null)
            {
                _logger.LogWarning("Clinician with Id {Id} not found", id);
                return NotFound();
            }

            var assignedPatientIds = await _context.Appointments
                .Where(a => a.ClinicianId == id)
                .Select(a => a.PatientId)
                .ToListAsync();

            var patients = await _context.Patients
                .Where(p => !assignedPatientIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    FullName = p.Name.FirstName + " " + p.Name.LastName
                })
                .ToListAsync();

            PatientsSelectList = new SelectList(patients, "Id", "FullName");

            AssignedPatients = await _context.Appointments
                .Where(a => a.ClinicianId == id)
                .Include(a => a.Patient)
                .ThenInclude(p => p.Name)
                .Select(a => new PatientAppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.Patient.Name.FirstName + " " + a.Patient.Name.LastName,
                    ReasonForVisit = a.ReasonForVisit
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSaveClinicianAsync()
        {
            _logger.LogInformation("OnPostSaveClinicianAsync called");

            ModelState.Remove("Reason");  // Remove validation for the Reason field

            if (!ModelState.IsValid || !ValidateClinician(Clinician))
            {
                _logger.LogWarning("Model state is invalid");

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        _logger.LogWarning("Error with {Key}: {Error}", modelStateKey, error.ErrorMessage);
                    }
                }

                return Page();
            }

            _logger.LogInformation("Attempting to update clinician with Id: {Id}", Clinician.Id);

            var clinicianInDb = await _context.Clinicians
                .Include(c => c.Name)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == Clinician.Id);

            if (clinicianInDb == null)
            {
                _logger.LogWarning("Clinician with Id {Id} not found", Clinician.Id);
                return NotFound();
            }

            // Update the properties of the existing clinician entity
            clinicianInDb.Name.FirstName = Clinician.Name.FirstName;
            clinicianInDb.Name.LastName = Clinician.Name.LastName;
            clinicianInDb.Address.Street = Clinician.Address.Street;
            clinicianInDb.Address.City = Clinician.Address.City;
            clinicianInDb.Address.State = Clinician.Address.State;
            clinicianInDb.Address.ZipCode = Clinician.Address.ZipCode;
            clinicianInDb.Specialty = Clinician.Specialty;
            clinicianInDb.PhoneNumber = Clinician.PhoneNumber;

            _context.Entry(clinicianInDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Clinician details updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error saving changes");

                if (!await ClinicianExists(Clinician.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Edit", new { id = Clinician.Id });
        }

        public async Task<IActionResult> OnPostAddPatientAsync()
        {
            _logger.LogInformation("OnPostAddPatientAsync called");

            ModelState.Remove("Clinician.Name");
            ModelState.Remove("Clinician.Address");
            ModelState.Remove("Clinician.Specialty");
            ModelState.Remove("Clinician.PhoneNumber");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        _logger.LogWarning("Error with {Key}: {Error}", modelStateKey, error.ErrorMessage);
                    }
                }

                return Page();
            }

            _logger.LogInformation("SelectedPatientId: {SelectedPatientId}, Reason: {Reason}", SelectedPatientId, Reason);

            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Now,
                ReasonForVisit = Reason,
                ClinicianId = Clinician.Id,
                PatientId = SelectedPatientId.Value
            };

            _context.Appointments.Add(appointment);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes");
                ModelState.AddModelError(string.Empty, "An error occurred while assigning the patient: " + ex.Message);
                return Page();
            }

            return RedirectToPage(new { id = Clinician.Id });
        }

        public async Task<IActionResult> OnPostDeletePatientAsync(Guid appointmentId, Guid clinicianId)
        {
            _logger.LogInformation("OnPostDeletePatientAsync called with appointment id: {AppointmentId}", appointmentId);

            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Appointment deleted successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving changes");
                    ModelState.AddModelError(string.Empty, "An error occurred while deleting the appointment: " + ex.Message);
                    return RedirectToPage(new { id = clinicianId });
                }
            }
            else
            {
                _logger.LogWarning("Appointment with Id {AppointmentId} not found", appointmentId);
            }

            return RedirectToPage(new { id = clinicianId });
        }

        public async Task<IActionResult> OnGetDownloadJsonAsync(Guid id)
        {
            _logger.LogInformation("OnGetDownloadJsonAsync called with id: {Id}", id);

            var clinician = await _context.Clinicians
                .Include(c => c.Name)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinician == null)
            {
                _logger.LogWarning("Clinician with Id {Id} not found", id);
                return NotFound();
            }

            var clinicianJson = JsonSerializer.Serialize(clinician);

            return new JsonResult(clinicianJson);
        }

        public async Task<IActionResult> OnPostUploadJsonAsync(Guid id, [FromBody] Clinician updatedClinician)
        {
            _logger.LogInformation("OnPostUploadJsonAsync called with id: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            var clinicianInDb = await _context.Clinicians
                .Include(c => c.Name)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinicianInDb == null)
            {
                _logger.LogWarning("Clinician with Id {Id} not found", id);
                return NotFound();
            }

            // Update the properties of the existing clinician entity
            clinicianInDb.Name.FirstName = updatedClinician.Name.FirstName;
            clinicianInDb.Name.LastName = updatedClinician.Name.LastName;
            clinicianInDb.Address.Street = updatedClinician.Address.Street;
            clinicianInDb.Address.City = updatedClinician.Address.City;
            clinicianInDb.Address.State = updatedClinician.Address.State;
            clinicianInDb.Address.ZipCode = updatedClinician.Address.ZipCode;
            clinicianInDb.Specialty = updatedClinician.Specialty;
            clinicianInDb.PhoneNumber = updatedClinician.PhoneNumber;

            _context.Entry(clinicianInDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Clinician details updated successfully via JSON upload.");
                return new JsonResult(new { success = true });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error saving changes via JSON upload.");

                if (!await ClinicianExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<bool> ClinicianExists(Guid id)
        {
            return await _context.Clinicians.AnyAsync(e => e.Id == id);
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
            }

            if (!addressRegex.IsMatch(clinician.Address.Street) || !addressRegex.IsMatch(clinician.Address.City) ||
                !addressRegex.IsMatch(clinician.Address.State) || !addressRegex.IsMatch(clinician.Address.ZipCode))
            {
                ModelState.AddModelError(string.Empty, "Address fields can only contain letters, numbers, spaces, commas, and hyphens.");
                isValid = false;
            }

            return isValid;
        }

        public class PatientAppointmentViewModel
        {
            public Guid AppointmentId { get; set; }
            public string PatientName { get; set; }
            public string ReasonForVisit { get; set; }
        }
    }
}
