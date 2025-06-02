using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using Microsoft.VisualBasic;

namespace FirstAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<int, Appointment> _appointmentRepository;
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Patient> _patientRepository;

        public AppointmentService(IRepository<int, Appointment> appointmentRepository,
                                  IRepository<int, Doctor> doctorRepository,
                                  IRepository<int, Patient> patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        public async Task<Appointment> Appointment(AppoitmentAddRequestDto appointment)
        {
            try
            {
                var newAppointment = new Appointment
                {
                    PatientId = appointment.PatientId,
                    DoctorId = appointment.DoctorId,
                    AppointmentDate = appointment.AppointmentDate,
                };

                newAppointment = await _appointmentRepository.Add(newAppointment);
                if (newAppointment == null)
                    throw new Exception("Could not add appointment");

                return newAppointment;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<ICollection<Appointment>> GetAppointmentsByPatient(int patientId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAll();
                return appointments.Where(a => a.PatientId == patientId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}