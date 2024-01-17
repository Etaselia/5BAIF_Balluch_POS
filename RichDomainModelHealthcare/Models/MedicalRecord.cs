namespace RichDomainModelHealthcare.Models {
    public class MedicalRecord {
        public Guid MedicalRecordId { get; set; }
        public Patient Patient { get; set; }
        public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
        public Guid PatientId { get; set; }   // Foreign key for Patient

        public MedicalRecord() {
            
        }
        // ... Additional properties and methods
        public void AddTreatment(Treatment treatment) {
            Treatments.Add(treatment);
        }

    }
}
