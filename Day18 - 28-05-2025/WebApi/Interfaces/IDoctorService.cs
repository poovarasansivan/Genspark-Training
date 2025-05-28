using WebApi.Models;
using WebApi.Interfaces;
using WebApi.Models.DTOs.DoctorSpecialities;

namespace WebApi.Interfaces
{
    public interface IDoctorService
    {
        public Task<Doctor> GetDoctByName(string name);
        public Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality);
        public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
    }
}