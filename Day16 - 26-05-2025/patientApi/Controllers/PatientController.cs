using PatientApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace PatientApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PatientController : ControllerBase
    {
      
        static List<Patient> patients = new List<Patient>
        {
            new Patient { Id = 1, Name = "John Doe", Age = 30, ReasonForVisit = "Checkup" },
            new Patient { Id = 2, Name = "Jane Smith", Age = 25, ReasonForVisit = "Flu" }
        };


        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            return Ok(patients);
        }

        [HttpPost]
        public ActionResult<Patient> CreatePatient([FromBody] Patient patient)
        {
     
            if (patient == null || string.IsNullOrEmpty(patient.Name) || patient.Age <= 0)
            {
                return BadRequest("Invalid patient data");
            }

            if (patients.Any(p => p.Id == patient.Id))
            {
                return BadRequest("A patient with this ID already exists");
            }

            patients.Add(patient);

            return Created("", patient);
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            patients.Remove(patient);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult<Patient> UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            if (updatedPatient == null || string.IsNullOrEmpty(updatedPatient.Name) || updatedPatient.Age <= 0)
            {
                return BadRequest("Invalid patient data");
            }

            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }

            patient.Name = updatedPatient.Name;
            patient.Age = updatedPatient.Age;
            patient.ReasonForVisit = updatedPatient.ReasonForVisit;

            return Ok(patient);
        }

        [HttpGet("{id}")]
        public ActionResult<Patient> GetById(int id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            return Ok(patient);
        }
    }
}
