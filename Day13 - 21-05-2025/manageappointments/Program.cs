using ManageAppointments.Interfaces;
using ManageAppointments.Models;
using ManageAppointments.Services;
using ManageAppointments.Repositories;

namespace ManageAppointments
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IRepository<int, Appointment> appointmentRepository = new AppointmentRepository();
            IAppointmentServices appointmentServices = new AppointmentService(appointmentRepository);
            ManageAppointments manageAppointments = new ManageAppointments(appointmentServices);

            manageAppointments.Start();
        }
    }
}
