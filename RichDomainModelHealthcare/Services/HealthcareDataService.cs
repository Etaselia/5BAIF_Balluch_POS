using RichDomainModelHealthcare.Data;
using RichDomainModelHealthcare.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RichDomainModelHealthcare.Services {
    public class HealthcareDataService {
        private readonly HealthcareContext _context;

        public HealthcareDataService(HealthcareContext context) {
            _context = context;
        }

        // Retrieves patients by a specific condition
        public IEnumerable<Patient> GetPatientsByCondition(Func<Patient, bool> condition) {
            return _context.Patients.Where(condition).ToList();
        }
        // Usage: var elderlyPatients = service.GetPatientsByCondition(p => p.DateOfBirth < DateTime.Now.AddYears(-65));

        public IEnumerable<Treatment> GetTreatmentsForPatient(Guid patientId) {
            return _context.Treatments.Where(t => t.PatientId == patientId).ToList();
        }
        // Usage: var treatments = service.GetTreatmentsForPatient(patientId);

        public IEnumerable<Appointment> GetAppointmentsForClinicianOnDate(Guid clinicianId, DateTime date) {
            return _context.Appointments.Where(a => a.ClinicianId == clinicianId && a.AppointmentDate.Date == date.Date).ToList();
        }
        // Usage: var appointments = service.GetAppointmentsForClinicianOnDate(clinicianId, DateTime.Now);

        public IEnumerable<Clinician> GetCliniciansBySpecialty(string specialty) {
            return _context.Clinicians.Where(c => c.Specialty.Equals(specialty, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        // Usage: var cardiologists = service.GetCliniciansBySpecialty("Cardiology");

        public int CountAppointmentsForPatient(Guid patientId) {
            return _context.Appointments.Count(a => a.PatientId == patientId);
        }
        // Usage: var appointmentCount = service.CountAppointmentsForPatient(patientId);

        // Additional methods...
        public IEnumerable<Patient> GetPatientsWithMultipleTreatments(int minTreatments) {
            return _context.Patients.Where(p => p.MedicalRecord.Treatments.Count >= minTreatments).ToList();
        }
        // Usage: var patientsWithMultipleTreatments = service.GetPatientsWithMultipleTreatments(5);

        public IEnumerable<Clinician> GetCliniciansWithNoAppointments() {
            return _context.Clinicians.Where(c => !c.Appointments.Any()).ToList();
        }
        // Usage: var availableClinicians = service.GetCliniciansWithNoAppointments();

        public IEnumerable<Invoice> GetAllInvoicesForPatient(Guid patientId) {
            return _context.Invoices.Where(i => i.Patient.Id == patientId).ToList();
        }
        // Usage: var patientInvoices = service.GetAllInvoicesForPatient(patientId);

        public IEnumerable<Treatment> GetRecentTreatments(DateTime sinceDate) {
            return _context.Treatments.Where(t => t.DateAdministered >= sinceDate).ToList();
        }
        // Usage: var recentTreatments = service.GetRecentTreatments(DateTime.Now.AddMonths(-1));

        public IEnumerable<Patient> GetPatientsByClinician(Guid clinicianId) {
            return _context.Appointments.Where(a => a.ClinicianId == clinicianId).Select(a => a.Patient).Distinct().ToList();
        }
        // Usage: var patientsOfClinician = service.GetPatientsByClinician(clinicianId);
        
        public void UpdateClinicianSpecialty(Guid clinicianId, string newSpecialty) {
            var clinician = _context.Clinicians.Find(clinicianId);
            if (clinician != null) {
                clinician.Specialty = newSpecialty;
                _context.SaveChanges();
            }
        }
        // Usage: service.UpdateClinicianSpecialty(clinicianId, "Neurology");

        public IEnumerable<Appointment> GetPastAppointmentsForPatient(Guid patientId) {
            return _context.Appointments.Where(a => a.PatientId == patientId && a.AppointmentDate < DateTime.Now).ToList();
        }
        // Usage: var pastAppointments = service.GetPastAppointmentsForPatient(patientId);
    }
}
