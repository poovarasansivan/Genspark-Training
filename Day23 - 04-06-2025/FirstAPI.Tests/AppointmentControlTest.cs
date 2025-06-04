using NUnit.Framework;
using Moq;
using FirstAPI.Controllers;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;

namespace FirstAPI.Test
{
    public class AppointmentControllerTest
    {
        private Mock<IAppointmentService> _appointmentServiceMock = null!;
        private AppointmentController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _controller = new AppointmentController(_appointmentServiceMock.Object);
        }

        [Test]
        public async Task PostAppointment_ReturnsCreated_WhenAppointmentCreated()
        {
            // Arrange
            var dto = new AppoitmentAddRequestDto();
            var appointment = new Appointment { AppointmentNumber = 1 };

            _appointmentServiceMock
                .Setup(s => s.Appointment(dto))
                .ReturnsAsync(appointment);

            // Act
            var result = await _controller.PostAppointment(dto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedResult>());
            var createdResult = (CreatedResult)result.Result;
            Assert.That(createdResult.Value, Is.EqualTo(appointment));
        }

        [Test]
        public async Task PostAppointment_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            // Arrange
            var dto = new AppoitmentAddRequestDto();

            _appointmentServiceMock
                .Setup(s => s.Appointment(dto))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _controller.PostAppointment(dto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.That(badRequest.Value, Is.EqualTo("Unable to process request at this moment"));
        }

        [Test]
        public async Task PostAppointment_ReturnsBadRequest_OnException()
        {
            // Arrange
            var dto = new AppoitmentAddRequestDto();

            _appointmentServiceMock
                .Setup(s => s.Appointment(dto))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.PostAppointment(dto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.That(badRequest.Value, Is.EqualTo("Test Exception"));
        }

        [Test]
        public async Task GetAppointmentsByPatient_ReturnsOk_WithAppointments()
        {
            // Arrange
            int patientId = 1;
            var appointments = new List<Appointment> { new Appointment { AppointmentNumber = 1 } };

            _appointmentServiceMock
                .Setup(s => s.GetAppointmentsByPatient(patientId))
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointmentsByPatient(patientId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.EqualTo(appointments));
        }

        [Test]
        public async Task GetAppointmentsByPatient_ReturnsBadRequest_OnException()
        {
            // Arrange
            int patientId = 1;

            _appointmentServiceMock
                .Setup(s => s.GetAppointmentsByPatient(patientId))
                .ThrowsAsync(new Exception("Test Error"));

            // Act
            var result = await _controller.GetAppointmentsByPatient(patientId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.That(badRequest.Value, Is.EqualTo("Test Error"));
        }

        [Test]
        public async Task GetAppointmentsByDoctor_ReturnsOk_WithAppointments()
        {
            // Arrange
            int doctorId = 1;
            var appointments = new List<Appointment> { new Appointment { AppointmentNumber = 1 } };

            _appointmentServiceMock
                .Setup(s => s.GetAppointmentsByDoctor(doctorId))
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointmentsByDoctor(doctorId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.EqualTo(appointments));
        }

        [Test]
        public async Task GetAppointmentsByDoctor_ReturnsBadRequest_OnException()
        {
            // Arrange
            int doctorId = 1;

            _appointmentServiceMock
                .Setup(s => s.GetAppointmentsByDoctor(doctorId))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.GetAppointmentsByDoctor(doctorId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.That(badRequest.Value, Is.EqualTo("Error"));
        }

        [Test]
        public async Task UpdateAppointmentStatus_ReturnsOk_WhenSuccess()
        {
            // Arrange
            int appointmentNumber = 1;
            var statusDto = new AppointmentStatusUpdateDto { Status = "Completed" };
            var updatedAppointment = new Appointment { AppointmentNumber = appointmentNumber };

            _appointmentServiceMock
                .Setup(s => s.UpdateAppointmentStatus(appointmentNumber, statusDto.Status, It.IsAny<string>()))
                .ReturnsAsync(updatedAppointment);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "poovarasan@gmail.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.UpdateAppointmentStatus(appointmentNumber, statusDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.EqualTo(updatedAppointment));
        }

        [Test]
        public async Task UpdateAppointmentStatus_ReturnsUnauthorized_WhenNoDoctorEmail()
        {
            // Arrange
            int appointmentNumber = 1;
            var statusDto = new AppointmentStatusUpdateDto { Status = "Completed" };

            // No claims setup for User
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.UpdateAppointmentStatus(appointmentNumber, statusDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<UnauthorizedObjectResult>());
            var unauthorizedResult = (UnauthorizedObjectResult)result.Result;
            Assert.That(unauthorizedResult.Value, Is.EqualTo("Doctor identity not found."));
        }

        [Test]
        public async Task UpdateAppointmentStatus_ReturnsNotFound_OnException()
        {
            // Arrange
            int appointmentNumber = 1;
            var statusDto = new AppointmentStatusUpdateDto { Status = "Completed" };

            _appointmentServiceMock
                .Setup(s => s.UpdateAppointmentStatus(appointmentNumber, statusDto.Status, It.IsAny<string>()))
                .ThrowsAsync(new Exception("Not found"));

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "doctor@example.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.UpdateAppointmentStatus(appointmentNumber, statusDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)result.Result;
            Assert.That(notFoundResult.Value, Is.EqualTo("Not found"));
        }
    }
}
