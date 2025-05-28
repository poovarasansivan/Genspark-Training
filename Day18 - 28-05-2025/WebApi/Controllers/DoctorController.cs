using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DoctorController : ControllerBase
    {
        static List<Doctor> doctors = new List<Doctor>
        {
            new Doctor { Id = 1, Name = "Dr. Smith" },
            new Doctor { Id = 2, Name = "Dr. Jones" }
        };


        [HttpGet]
        public ActionResult<IEnumerable<Doctor>> GetDoctors()
        {
            return Ok(doctors);
        }

        [HttpPost]
        public ActionResult<Doctor> CreateDoctor([FromBody] Doctor doctor)
        {
            if (doctor == null || string.IsNullOrEmpty(doctor.Name))
            {
                return BadRequest("Invalid doctor data");
            }

            doctors.Add(doctor);
            return Created("",doctor);
        }

        [HttpPut("{id}")]
        public ActionResult<Doctor> UpdateDoctor(int id, [FromBody] Doctor updatedDoctor)
        {
            if (updatedDoctor == null || string.IsNullOrEmpty(updatedDoctor.Name))
            {
                return BadRequest("Invalid doctor data");
            }

            var doctor = doctors.Find(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }

            doctor.Name = updatedDoctor.Name;
            return Ok(doctor);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteDoctor(int id)
        {
            var doctor = doctors.Find(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }

            doctors.Remove(doctor);
            return NoContent();
        }


        [HttpGet("{id}")]
        public ActionResult<Doctor> GetDoctor(int id)
        {
            var doctor = doctors.Find(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }
            return Ok(doctor);
        }
    }
}