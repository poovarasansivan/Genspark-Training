using WebApi.Models;
using WebApi.Models.DTOs.Appointments;

namespace WebApi.Interfaces
{
  public interface IAppointmentService
  {
    Task<Appointment> GetAppointmentById(int id);
    Task<ICollection<Appointment>> GetAppointmentsByPatientId(int patientId);
    Task<ICollection<Appointment>> GetAppointmentsByDoctorId(int doctorId);
    Task<Appointment> AddAppointment(AppointmentAddRequestDto appointment);
    Task<bool> UpdateAppointmentStatus(int appointmentId, string status);
    Task<bool> DeleteAppointment(int appointmentId);
  }   
}
