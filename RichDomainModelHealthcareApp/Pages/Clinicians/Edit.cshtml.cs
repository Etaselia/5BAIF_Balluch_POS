using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    public class EditModel : PageModel
    {
        private readonly HealthcareContext _context;

        public EditModel(HealthcareContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClinicianAppointmentViewModel AppointmentDetails { get; set; }

        public SelectList PatientsSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var clinician = await _context.Clinicians.FindAsync(id);
            if (clinician == null)
            {
                return NotFound();
            }

            AppointmentDetails = new ClinicianAppointmentViewModel
            {
                ClinicianId = id
                // You may wish to populate the specialty or other properties if needed
            };

            var patients = await _context.Patients
                                .Select(p => new { p.Id, p.Name })
                                .ToListAsync();

            PatientsSelectList = new SelectList(patients, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var clinician = await _context.Clinicians.FindAsync(AppointmentDetails.ClinicianId);
            if (clinician == null)
            {
                return NotFound();
            }

            if (AppointmentDetails.SelectedPatientId.HasValue)
            {
                var patient = await _context.Patients.FindAsync(AppointmentDetails.SelectedPatientId.Value);
                if (patient != null)
                {
                    // Assuming you have a method AddAppointment on your clinician entity
                    clinician.AddAppointment(new Appointment(
                        DateTime.Now, 
                        clinician, 
                        patient, 
                        AppointmentDetails.Reason));

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("./Index");
        }
    }

    public class ClinicianAppointmentViewModel
    {
        public Guid ClinicianId { get; set; }
        public Guid? SelectedPatientId { get; set; }
        public string Reason { get; set; } // The reason for the appointment
    }
}
