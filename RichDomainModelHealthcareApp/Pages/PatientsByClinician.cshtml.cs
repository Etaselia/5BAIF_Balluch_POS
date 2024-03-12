using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Data; // Use the correct namespace
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PatientsByClinicianModel : PageModel
{
    private readonly HealthcareContext _context;

    public PatientsByClinicianModel(HealthcareContext context)
    {
        _context = context;
    }

    // Define a ViewModel to represent each group of patients for a clinician
    public class ClinicianPatientsGroup
    {
        public string ClinicianName { get; set; }
        public List<string> PatientNames { get; set; }
    }

    public IList<ClinicianPatientsGroup> ClinicianPatientsGroups { get; set; }

    public async Task OnGetAsync()
    {
        ClinicianPatientsGroups = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Clinician)
            // Optional: Include additional navigation properties if needed
            .Select(a => new { a.Clinician, a.Patient })
            .GroupBy(ap => ap.Clinician.Id)
            .Select(group => new ClinicianPatientsGroup
            {
                ClinicianName = group.First().Clinician.Name.FirstName + " " + group.First().Clinician.Name.LastName, // Adjust according to your Name structure
                PatientNames = group.Select(g => g.Patient.Name.FirstName + " " + g.Patient.Name.LastName).ToList() // Adjust according to your Name structure
            })
            .ToListAsync();
    }
}