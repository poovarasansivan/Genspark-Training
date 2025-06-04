using NUnit.Framework;
using Moq;
using FirstAPI.Controllers;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace FirstAPI.Test
{
    public class PatientControllerTests
    {
        private Mock<IPatientService> _patientServiceMock = null!;
        private Mock<PatientsMapper> _patientsMapperMock = null!;
        private PatientController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _patientsMapperMock = new Mock<PatientsMapper>();
            _controller = new PatientController(_patientServiceMock.Object, _patientsMapperMock.Object);
        }

        [Test]
        public async Task GetPatient_ReturnsOk_WhenPatientFound()
        {
            // Arrange
            var name = "John Doe";
            var patient = new Patient
            {
                Id = 1,
                Name = name,
                Age = 30,
                Email = "poovarasan@gmail.com",
                
            };

            _patientServiceMock
                .Setup(s => s.GetPatientByName(name))
                .ReturnsAsync(patient);

            // Act
            var result = await _controller.GetPatient(name);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result.Result;
            Assert.That(okResult.Value, Is.EqualTo(patient));
        }

        [Test]
        public async Task GetPatient_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            var name = "Unknown";

            _patientServiceMock
                .Setup(s => s.GetPatientByName(name))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.GetPatient(name);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = (NotFoundObjectResult)result.Result;
            Assert.That(notFoundResult.Value, Is.EqualTo("Patient not found"));
        }

        [Test]
        public async Task PostPatient_ReturnsCreated_WhenPatientAdded()
        {
            // Arrange
            var patientDto = new PatientAddRequestDto
            {
                // Populate properties as needed
            };
            var addedPatient = new Patient
            {
                Id = 1,
                // Populate properties as needed
            };

            _patientServiceMock
                .Setup(s => s.AddPatient(patientDto))
                .ReturnsAsync(addedPatient);

            // Act
            var result = await _controller.PostPatient(patientDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<CreatedResult>());
            var createdResult = (CreatedResult)result.Result;
            Assert.That(createdResult.Value, Is.EqualTo(addedPatient));
        }

        [Test]
        public async Task PostPatient_ReturnsBadRequest_WhenAddPatientReturnsNull()
        {
            // Arrange
            var patientDto = new PatientAddRequestDto();

            _patientServiceMock
                .Setup(s => s.AddPatient(patientDto))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.PostPatient(patientDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.That(badRequestResult.Value, Is.EqualTo("Unable to process request at this moment"));
        }

        [Test]
        public async Task PostPatient_ReturnsBadRequest_OnException()
        {
            // Arrange
            var patientDto = new PatientAddRequestDto();

            _patientServiceMock
                .Setup(s => s.AddPatient(patientDto))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.PostPatient(patientDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.That(badRequestResult.Value, Is.EqualTo("Test exception"));
        }
    }
}
