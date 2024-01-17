using RichDomainModelHealthcare.Interfaces;
using RichDomainModelHealthcare.Models;

namespace RichDomainModelHealthcare.Services {
    public class AppointmentService {
        private readonly IRepository<Appointment> _appointmentRepository;

        public AppointmentService(IRepository<Appointment> appointmentRepository) {
            _appointmentRepository = appointmentRepository;
        }

        public async Task ScheduleAppointmentAsync(Appointment appointment) {
            // Add business logic for scheduling an appointment
            await _appointmentRepository.CreateAsync(appointment);
        }

        // ... Additional service methods
    }
}
