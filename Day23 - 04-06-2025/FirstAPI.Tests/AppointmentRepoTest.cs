using NUnit.Framework;
using FirstAPI.Repositories;
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FirstAPI.Test
{
    public class AppointmentRepositoryTest
    {
        private ClinicContext _context = null!;
        private AppointmentRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ClinicContext(options);
            _repository = new AppointmentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_ReturnsAppointment_WhenAppointmentExists()
        {
            // Arrange
            var appointment = new Appointment
            {
                AppointmentNumber = 1,
                PatientId = 101,
                DoctorId = 201,
                AppointmentDate = DateTime.Now,
                Status = "Pending"
            };
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.Get(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AppointmentNumber, Is.EqualTo(1));
            Assert.That(result.PatientId, Is.EqualTo(101));
            Assert.That(result.DoctorId, Is.EqualTo(201));
            Assert.That(result.Status, Is.EqualTo("Pending"));
        }

        [Test]
        public void Get_ThrowsException_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var nonExistingId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Get(nonExistingId));
            Assert.That(ex?.Message, Is.EqualTo("No Appointment with the given ID"));
        }

        [Test]
        public async Task GetAll_ReturnsAllAppointments_WhenAppointmentsExist()
        {
    
            var appointments = new List<Appointment>
            {
                new Appointment 
                { 
                    AppointmentNumber = 1, 
                    PatientId = 101, 
                    DoctorId = 201, 
                    AppointmentDate = DateTime.Now, 
                    Status = "Pending" 
                },
                new Appointment 
                { 
                    AppointmentNumber = 2, 
                    PatientId = 102, 
                    DoctorId = 202, 
                    AppointmentDate = DateTime.Now.AddDays(1), 
                    Status = "Completed" 
                }
            };
            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            var results = await _repository.GetAll();

            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAll_ThrowsException_WhenNoAppointmentsExist()
        {
         
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.That(ex?.Message, Is.EqualTo("No Appointment in the database"));
        }
    }
}
