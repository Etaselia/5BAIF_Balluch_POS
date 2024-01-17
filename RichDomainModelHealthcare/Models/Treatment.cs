using RichDomainModelHealthcare.Models.ValueObjects;

namespace RichDomainModelHealthcare.Models {
    public class Treatment {
        public Guid TreatmentId { get; set; }
        public string Description { get; set; }
        public DateTime DateAdministered { get; set; }
        public Clinician Clinician { get; set; }
        public Guid ClinitianId { get; set;}
        public Patient Patient { get; set; }
        public Guid PatientId { get; set; }
        public Money Cost { get; set; }
        public Guid MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public Treatment() {
            
        }
        
        public void AddInvoice(Invoice invoice) {
            Invoices.Add(invoice);
        }
        // ... Additional properties and methods
    }
}
