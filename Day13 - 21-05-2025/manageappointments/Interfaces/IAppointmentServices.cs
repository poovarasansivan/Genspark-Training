using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageAppointments.Models;

namespace ManageAppointments.Interfaces
{
    public interface IAppointmentServices
    {
        int AddAppointment(Appointment appointment);
        List<Appointment>? SearchAppointment(AppointmentSearchModel searchModel);
    }
}