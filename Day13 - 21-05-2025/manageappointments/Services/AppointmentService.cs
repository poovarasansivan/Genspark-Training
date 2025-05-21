using ManageAppointments.Interfaces;
using ManageAppointments.Models;
using ManageAppointments.Exceptions;


namespace ManageAppointments.Services
{
    public class AppointmentService : IAppointmentServices
    {
        IRepository<int, Appointment> _repository;

        public AppointmentService(IRepository<int, Appointment> repository)
        {
            _repository = repository;
        }

        public int AddAppointment(Appointment appointment)
        {
            try
            {
                var result = _repository.Add(appointment);
                if (result == null)
                {
                    throw new Exception("Failed to add appointment");
                }
                return result.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding appointment", ex);
            }
            return -1;
        }

        public List<Appointment>? SearchAppointment(AppointmentSearchModel searchModel)
        {
            try
            {
                var appointments = _repository.GetAll();

                appointments = SearchByName(appointments, searchModel.PatientName ?? string.Empty);
                appointments = SearchByDate(appointments, searchModel.AppointmentDate);
                appointments = SearchByAge(appointments, searchModel.Age ?? new RangeInt<int> { minVal = 0, maxVal = 120 });
                if (appointments != null && appointments.Count > 0)
                {
                    return appointments.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error searching appointments: " + ex.Message);
            }

            return null;
        }

        private ICollection<Appointment> SearchByName(ICollection<Appointment> appointments, string name)
        {
            if (name == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.PatientName != null && a.PatientName.ToLower().Contains(name.ToLower())).ToList();
            }
        }

        private ICollection<Appointment> SearchByDate(ICollection<Appointment> appointments, DateTime? appointmentDate)
        {
            if (appointmentDate == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }

            return appointments
                .Where(a => a.AppointmentDate.Date == appointmentDate.Value.Date)
                .ToList();
        }


        private ICollection<Appointment> SearchByAge(ICollection<Appointment> appointments, RangeInt<int> ageRange)
        {
            if (appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            else
            {
                return appointments.Where(a => a.PatientAge >= ageRange.minVal && a.PatientAge <= ageRange.maxVal).ToList();
            }
        }
    }
}