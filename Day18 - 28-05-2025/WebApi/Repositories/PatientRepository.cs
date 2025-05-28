using WebApi.Contexts;
using WebApi.Interfaces;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repositories
{
    public  class PatinetRepository : Repository<int, Patient>
    {
        protected PatinetRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Patient> Get(int key)
        {
            var patient = await _clinicContext.Patients.SingleOrDefaultAsync(p => p.Id == key);

            return patient??throw new Exception("No patient with teh given ID");
        }

        public override async Task<IEnumerable<Patient>> GetAll()
        {
            var patients = _clinicContext.Patients;
            if (patients.Count() == 0)
                throw new Exception("No Patients in the database");
            return (await patients.ToListAsync());
        }
    }
}