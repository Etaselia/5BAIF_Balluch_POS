using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;

namespace RichDomainModelHealthcareApp.Pages.Clinicians;

public class ClinicianDetailsModel : PageModel
{
    private readonly HealthcareContext _context;

    public Person? Clinician { get; private set; }

    public ClinicianDetailsModel(HealthcareContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Clinician = await _context.Set<Person>()
            .Include(p => p.Name)
            .Include(p => p.Address)
            .FirstOrDefaultAsync(p => p.Id == id && p is Clinician);

        if (Clinician == null)
        {
            return NotFound();
        }

        return Page();
    }
}