using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Appointment>> PostAppointment([FromBody] AppoitmentAddRequestDto appointment)
        {
            try
            {
                var newAppointment = await _appointmentService.Appointment(appointment);
                if (newAppointment != null)
                    return Created("", newAppointment);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{patientId}")]
        [Authorize]
        public async Task<ActionResult<ICollection<Appointment>>> GetAppointmentsByPatient(int patientId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPatient(patientId);
                return Ok(appointments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}