namespace RichDomainModelHealthcare.Models {
    public class MedicalRecord {
        public Guid MedicalRecordId { get; set; }
        public Patient Patient { get; set; }
        private List<Treatment> _treatments = new List<Treatment>();
        public Guid PatientId { get; set; } // Foreign key for Patient

        public virtual IReadOnlyCollection<Treatment> Treatments => _treatments.AsReadOnly();

        public MedicalRecord() { }

        public void AddTreatment(Treatment treatment) {
            _treatments.Add(treatment);
        }
        // ... Additional properties and methods
    }
}
