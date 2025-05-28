using WebApi.Models;


namespace WebApi.Interfaces
{
    public interface IpatientService
    {
        Task<Patient> GetPatientById(int patientId);
        Task<ICollection<Patient>> GetPatientsByName(string name);
        Task<Patient> AddPatient(Patient patient);
        Task<Patient> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(int patientId);
    }
}