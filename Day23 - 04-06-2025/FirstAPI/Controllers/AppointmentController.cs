using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Patient")]
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
        [Authorize(Roles = "Patient")]
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

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ICollection<Appointment>>> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctor(doctorId);
                return Ok(appointments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{appointmentNumber}")]
        [Authorize(Roles = "Doctor", Policy = "ExperiencedDoctorOnly")]
        public async Task<ActionResult<Appointment>> UpdateAppointmentStatus(int appointmentNumber, [FromBody] AppointmentStatusUpdateDto statusDto)
        {
            try
            {
                var doctorEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorEmail))
                    return Unauthorized("Doctor identity not found.");

                var updatedAppointment = await _appointmentService.UpdateAppointmentStatus(appointmentNumber, statusDto.Status, doctorEmail);
                return Ok(updatedAppointment);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }



    }
}