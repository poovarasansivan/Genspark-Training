using WebApi.Contexts;
using WebApi.Interfaces;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repositories
{
    public  class AppointmentRepository : Repository<string, Appointment>
    {
        protected AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Appointment> Get(string key)
        {
            var appointment = await _clinicContext.Appointments.SingleOrDefaultAsync(p => p.AppointmentNumber == key);

            return appointment??throw new Exception("No appointmnet with the given ID");
        }

        public override async Task<IEnumerable<Appointment>> GetAll()
        {
            var appointments = _clinicContext.Appointments;
            if (appointments.Count() == 0)
                throw new Exception("No Appointment in the database");
            return (await appointments.ToListAsync());
        }
    }
}