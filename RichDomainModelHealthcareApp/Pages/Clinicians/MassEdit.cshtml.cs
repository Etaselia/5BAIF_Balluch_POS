using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RichDomainModelHealthcare.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RichDomainModelHealthcare.Models;

namespace RichDomainModelHealthcareApp.Pages.Clinicians
{
    public class MassEditModel : PageModel
    {
        private readonly HealthcareContext _context;

        [BindProperty]
        public Guid SelectedPatientId { get; set; }

        [BindProperty]
        public List<ClinicianSelection> Clinicians { get; set; }

        public SelectList PatientsSelectList { get; set; }

        public MassEditModel(HealthcareContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            var patients = await _context.Patients
                                .Select(p => new { p.Id, FullName = p.Name.FirstName + " " + p.Name.LastName })
                                .ToListAsync();

            PatientsSelectList = new SelectList(patients, "Id", "FullName");

            Clinicians = await _context.Clinicians
                .Select(c => new ClinicianSelection 
                { 
                    Id = c.Id, 
                    Name = c.Name.FirstName + " " + c.Name.LastName,
                    IsSelected = false 
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var clinician in Clinicians.Where(c => c.IsSelected))
                    {
                        if (!await ClinicianExists(clinician.Id))
                        {
                            return NotFound($"Clinician with ID {clinician.Id} not found.");
                        }

                        var appointment = new Appointment
                        {
                            ClinicianId = clinician.Id,
                            PatientId = SelectedPatientId,
                            AppointmentDate = DateTime.Now,
                            ReasonForVisit = clinician.ReasonForVisit
                        };

                        _context.Appointments.Add(appointment);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred saving the appointments.");
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> ClinicianExists(Guid id)
        {
            return await _context.Clinicians.AnyAsync(e => e.Id == id);
        }
    }

    public class ClinicianSelection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string ReasonForVisit { get; set; }
    }
}
