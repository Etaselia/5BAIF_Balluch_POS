namespace RichDomainModelHealthcare.Models {
    public class Appointment {
        public Guid AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public Clinician Clinician { get; set; }
        public Patient Patient { get; set; }
        public string ReasonForVisit { get; set; }
        public Guid ClinicianId { get; set; } // Foreign key for Clinician
        public Guid PatientId { get; set; }   // Foreign key for Patient

        // Needed for entity framework, fuck me, took me a while to figure out
        public Appointment()
        {
        }
        public Appointment(DateTime appointmentDate, Clinician clinician, Patient patient, string reason) {
            AppointmentId = Guid.NewGuid();
            AppointmentDate = appointmentDate;
            Clinician = clinician;
            Patient = patient;
            ReasonForVisit = reason;
        }
        
        public void RescheduleAppointment(DateTime newDate) {
            AppointmentDate = newDate;
        }

    }
}
