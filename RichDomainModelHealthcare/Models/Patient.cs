using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Models {
    public class Patient : Person {
        public Address Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        private List<Appointment> _appointments = new List<Appointment>();
        private List<Invoice> _invoices = new List<Invoice>();
        public MedicalRecord MedicalRecord { get; set; }

        public virtual IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();
        public virtual IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

        public Patient() : base(null, null, default(DateTime), null) {
        }
        public Patient(Name name, Address address, DateTime dob, string phone) 
            : base(name, address, dob, phone) { }

        public void AddAppointment(Appointment appointment) {
            _appointments.Add(appointment);
        }

        public void AddInvoice(Invoice invoice) {
            _invoices.Add(invoice);
        }
    }
}