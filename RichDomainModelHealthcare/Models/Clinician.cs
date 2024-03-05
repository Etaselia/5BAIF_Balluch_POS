using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Models {
    public class Clinician : Person {
        public string Specialty { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();

        public Clinician() : base(null, null, default(DateTime), null) {
        }
        // Constructor
        public Clinician(Name name, Address address, DateTime dateOfBirth, string phoneNumber, string specialty)
            : base(name, address, dateOfBirth, phoneNumber) {
            Specialty = specialty;
        }
        
        public void AddAppointment(Appointment appointment) => Appointments.Add(appointment);
        public void AddTreatment(Treatment treatment) => Treatments.Add(treatment);

        // Additional methods specific to Clinician
        public IEnumerable<Patient> GetTreatedPatients() {
            return Treatments.Select(t => t.Patient).Distinct();
        }
    }
}