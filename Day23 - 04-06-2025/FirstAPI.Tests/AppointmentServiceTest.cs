using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAPI.Test
{
    [TestFixture]
    public class AppointmentServiceTests
    {
        private Mock<IRepository<int, Appointment>> _mockAppointmentRepo;
        private Mock<IRepository<int, Doctor>> _mockDoctorRepo;
        private Mock<IRepository<int, Patient>> _mockPatientRepo;
        private AppointmentService _service;

        [SetUp]
        public void Setup()
        {
            _mockAppointmentRepo = new Mock<IRepository<int, Appointment>>();
            _mockDoctorRepo = new Mock<IRepository<int, Doctor>>();
            _mockPatientRepo = new Mock<IRepository<int, Patient>>();

            _service = new AppointmentService(
                _mockAppointmentRepo.Object,
                _mockDoctorRepo.Object,
                _mockPatientRepo.Object);
        }

        [Test]
        public async Task Appointment_ValidData_AddsAndReturnsAppointment()
        {
            // Arrange
            var dto = new AppoitmentAddRequestDto
            {
                PatientId = 1,
                DoctorId = 2,
                AppointmentDate = DateTime.Today.AddDays(1),
                Status = null
            };

            _mockAppointmentRepo.Setup(r => r.GetAll())
                .ReturnsAsync(new List<Appointment>());

            _mockAppointmentRepo.Setup(r => r.Add(It.IsAny<Appointment>()))
                .ReturnsAsync((Appointment a) => a);

            // Act
            var result = await _service.Appointment(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PatientId, Is.EqualTo(dto.PatientId));
            Assert.That(result.DoctorId, Is.EqualTo(dto.DoctorId));
            Assert.That(result.Status, Is.EqualTo("Pending")); // Default fallback
            Assert.That(result.AppointmentNumber, Is.InRange(10000, 99999));

            _mockAppointmentRepo.Verify(r => r.GetAll(), Times.AtLeastOnce);
            _mockAppointmentRepo.Verify(r => r.Add(It.IsAny<Appointment>()), Times.Once);
        }

        [Test]
        public async Task GetAppointmentsByPatient_ReturnsCorrectAppointments()
        {
            // Arrange
            var patientId = 5;
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentNumber = 1, PatientId = patientId, DoctorId = 2 },
                new Appointment { AppointmentNumber = 2, PatientId = 3, DoctorId = 4 },
                new Appointment { AppointmentNumber = 3, PatientId = patientId, DoctorId = 5 }
            };

            _mockAppointmentRepo.Setup(r => r.GetAll()).ReturnsAsync(appointments);

            // Act
            var result = await _service.GetAppointmentsByPatient(patientId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(a => a.PatientId == patientId), Is.True);
        }

        [Test]
        public async Task GetAppointmentsByDoctor_ReturnsPendingAppointmentsForDoctor()
        {
            // Arrange
            var doctorId = 7;
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentNumber = 1, PatientId = 1, DoctorId = doctorId, Status = "Pending" },
                new Appointment { AppointmentNumber = 2, PatientId = 2, DoctorId = doctorId, Status = "Completed" },
                new Appointment { AppointmentNumber = 3, PatientId = 3, DoctorId = doctorId, Status = "pending" },
                new Appointment { AppointmentNumber = 4, PatientId = 4, DoctorId = 8, Status = "Pending" }
            };

            _mockAppointmentRepo.Setup(r => r.GetAll()).ReturnsAsync(appointments);

            // Act
            var result = await _service.GetAppointmentsByDoctor(doctorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(a => a.DoctorId == doctorId && a.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        [Test]
        public async Task UpdateAppointmentStatus_ValidData_UpdatesStatus()
        {
            // Arrange
            int appointmentNumber = 12345;
            string newStatus = "Completed";
            string doctorEmail = "poovarasan@gmail.com";

            var appointment = new Appointment { AppointmentNumber = appointmentNumber, DoctorId = 10, Status = "Pending" };
            var doctor = new Doctor { Id = 10, Email = doctorEmail };

            _mockAppointmentRepo.Setup(r => r.Get(appointmentNumber)).ReturnsAsync(appointment);
            _mockDoctorRepo.Setup(r => r.Get(appointment.DoctorId)).ReturnsAsync(doctor);
            _mockAppointmentRepo.Setup(r => r.Update(appointmentNumber, It.IsAny<Appointment>()))
                .ReturnsAsync((int id, Appointment a) => a);

            // Act
            var updated = await _service.UpdateAppointmentStatus(appointmentNumber, newStatus, doctorEmail);

            // Assert
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.Status, Is.EqualTo(newStatus));

            _mockAppointmentRepo.Verify(r => r.Get(appointmentNumber), Times.Once);
            _mockDoctorRepo.Verify(r => r.Get(appointment.DoctorId), Times.Once);
            _mockAppointmentRepo.Verify(r => r.Update(appointmentNumber, It.IsAny<Appointment>()), Times.Once);
        }

        [Test]
        public void UpdateAppointmentStatus_InvalidAppointment_ThrowsException()
        {
            // Arrange
            int appointmentNumber = 9999;
            string newStatus = "Cancelled";
            string doctorEmail = "poovarasan@gmail.com";

            _mockAppointmentRepo.Setup(r => r.Get(appointmentNumber)).ReturnsAsync((Appointment)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.UpdateAppointmentStatus(appointmentNumber, newStatus, doctorEmail));

            Assert.That(ex.Message, Is.EqualTo("Appointment not found"));
        }

        [Test]
        public void UpdateAppointmentStatus_UnauthorizedDoctor_ThrowsException()
        {
            // Arrange
            int appointmentNumber = 5555;
            string newStatus = "Cancelled";
            string doctorEmail = "poovarasan@gmail.com";

            var appointment = new Appointment { AppointmentNumber = appointmentNumber, DoctorId = 10, Status = "Pending" };
            var doctor = new Doctor { Id = 10, Email = "poovaasan@gmail.com" };

            _mockAppointmentRepo.Setup(r => r.Get(appointmentNumber)).ReturnsAsync(appointment);
            _mockDoctorRepo.Setup(r => r.Get(appointment.DoctorId)).ReturnsAsync(doctor);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.UpdateAppointmentStatus(appointmentNumber, newStatus, doctorEmail));

            Assert.That(ex.Message, Is.EqualTo("Unauthorized: This appointment does not belong to the logged-in doctor."));
        }
    }
}
