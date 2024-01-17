using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Models {
    public class Patient {
        public Guid PatientId { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public MedicalRecord MedicalRecord { get; set; }

        public Patient() {
            
        }
        public Patient(Name name, Address address, DateTime dob, string phone) {
            PatientId = Guid.NewGuid();
            Name = name;
            Address = address;
            DateOfBirth = dob;
            PhoneNumber = phone;
        }

        public void AddAppointment(Appointment appointment) => Appointments.Add(appointment);
        public void AddTreatment(Treatment treatment) => Treatments.Add(treatment);
        public void AddInvoice(Invoice invoice) => Invoices.Add(invoice);
        
        
        public IEnumerable<Treatment> GetAllPastTreatments() {
            return MedicalRecord.Treatments
                .Where(t => t.DateAdministered <= DateTime.Now)
                .ToList();
        }
    }
}
