using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    public class EditModel : PageModel
    {
        private readonly HealthcareContext _context;

        [BindProperty]
        public ClinicianAppointmentViewModel AppointmentDetails { get; set; }

        public SelectList PatientsSelectList { get; set; }

        public EditModel(HealthcareContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var clinician = await _context.Clinicians.FirstOrDefaultAsync(c => c.Id == id);
            if (clinician == null)
            {
                return NotFound();
            }

            AppointmentDetails = new ClinicianAppointmentViewModel
            {
                ClinicianId = id
                // Add more fields if needed
            };

            var patients = await _context.Patients
                                .Select(p => new 
                                { 
                                    p.Id, 
                                    FullName = p.Name.FirstName + " " + p.Name.LastName 
                                })
                                .ToListAsync();

            PatientsSelectList = new SelectList(patients, "Id", "FullName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (AppointmentDetails.SelectedPatientId.HasValue)
            {
                var appointment = new Appointment
                {
                    AppointmentDate = DateTime.Now, // Or another appropriate time
                    ReasonForVisit = AppointmentDetails.Reason,
                    ClinicianId = AppointmentDetails.ClinicianId,
                    PatientId = AppointmentDetails.SelectedPatientId.Value
                };

                _context.Appointments.Add(appointment);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ClinicianExists(AppointmentDetails.ClinicianId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> ClinicianExists(Guid id)
        {
            return await _context.Clinicians.AnyAsync(e => e.Id == id);
        }
    }

    public class ClinicianAppointmentViewModel
    {
        public Guid ClinicianId { get; set; }
        public Guid? SelectedPatientId { get; set; }
        public string Reason { get; set; }
    }
}
