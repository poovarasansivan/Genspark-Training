using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FirstAPI.Services;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly PatientsMapper _patientsMapper;

        public PatientController(IPatientService patientService, PatientsMapper patientsMapper)
        {
            _patientService = patientService;
            _patientsMapper = patientsMapper;
        }

        [HttpGet("{name}")]
        [Authorize]
        public async Task<ActionResult<Patient>> GetPatient(string name)
        {
            var result = await _patientService.GetPatientByName(name);
            if (result != null)
                return Ok(result);
            return NotFound("Patient not found");
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient([FromBody] PatientAddRequestDto patient)
        {
            try
            {
                var addedPatient = await _patientService.AddPatient(patient);
                if (addedPatient != null)
                    return Created("", addedPatient);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}