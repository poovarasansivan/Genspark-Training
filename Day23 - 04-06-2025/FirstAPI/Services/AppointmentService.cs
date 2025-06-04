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
                var rnd = new Random();
                int randomAppointmentNumber;
                bool isUnique = false;

                do
                {
                    randomAppointmentNumber = rnd.Next(10000, 100000);
                    var existingAppointments = await _appointmentRepository.GetAll();
                    isUnique = !existingAppointments.Any(a => a.AppointmentNumber == randomAppointmentNumber);

                } while (!isUnique);

                var newAppointment = new Appointment
                {
                    PatientId = appointment.PatientId,
                    DoctorId = appointment.DoctorId,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentNumber = randomAppointmentNumber,
                    Status = appointment.Status ?? "Pending",
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

        public async Task<ICollection<Appointment>> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAll();
                return appointments
                    .Where(a => a.DoctorId == doctorId && a.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Appointment> UpdateAppointmentStatus(int appointmentNumber, string status, string doctorEmail)
        {
            var appointment = await _appointmentRepository.Get(appointmentNumber);
            if (appointment == null)
                throw new Exception("Appointment not found");

            var doctor = await _doctorRepository.Get(appointment.DoctorId);
            if (doctor == null || !doctor.Email.Equals(doctorEmail, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Unauthorized: This appointment does not belong to the logged-in doctor.");

            appointment.Status = status;
            return await _appointmentRepository.Update(appointmentNumber, appointment);
        }


    }
}