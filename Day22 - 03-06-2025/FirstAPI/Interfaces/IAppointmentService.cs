using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;


namespace FirstAPI.Interfaces
{
    public interface IAppointmentService
    {

        public Task<Appointment> Appointment(AppoitmentAddRequestDto appointment);
        public Task<ICollection<Appointment>> GetAppointmentsByPatient(int patientId);

        public Task<ICollection<Appointment>> GetAppointmentsByDoctor(int doctorId);
        public Task<Appointment> UpdateAppointmentStatus(int appointmentNumber, string newStatus);
    }
}