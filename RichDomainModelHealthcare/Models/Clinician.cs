using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Models {
    public class Clinician {
        public Guid ClinicianId { get; set; }
        public Name Name { get; set; }
        public string Specialty { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();


        public Clinician() {
            
        }
        public Clinician(Name name, string specialty) {
            ClinicianId = Guid.NewGuid();
            Name = name;
            Specialty = specialty;
        }

        public void AddAppointment(Appointment appointment) => Appointments.Add(appointment);
        public void AddTreatment(Treatment treatment) => Treatments.Add(treatment);

        // ... Additional properties and methods
    }
}
