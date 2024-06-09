using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    public class ClinicianDetailsModel : PageModel
    {
        private readonly HealthcareContext _context;

        public ClinicianDetailsModel(HealthcareContext context)
        {
            _context = context;
        }

        public Clinician? Clinician { get; private set; }
        public IList<PatientAppointment> PatientAppointments { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Clinician = await _context.Clinicians
                .Include(c => c.Name)
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Clinician == null)
            {
                return NotFound();
            }

            PatientAppointments = await _context.Appointments
                .Where(a => a.ClinicianId == id)
                .Include(a => a.Patient)
                .ThenInclude(p => p.Name)
                .Select(a => new PatientAppointment
                {
                    PatientName = a.Patient.Name.FirstName + " " + a.Patient.Name.LastName,
                    ReasonForVisit = a.ReasonForVisit
                })
                .ToListAsync();

            // Debugging: Log the number of patient appointments
            System.Diagnostics.Debug.WriteLine($"Clinician: {Clinician.Name.FirstName} {Clinician.Name.LastName}");
            System.Diagnostics.Debug.WriteLine($"Number of Patient Appointments: {PatientAppointments.Count}");

            return Page();
        }

        public class PatientAppointment
        {
            public string PatientName { get; set; }
            public string ReasonForVisit { get; set; }
        }
    }
}
