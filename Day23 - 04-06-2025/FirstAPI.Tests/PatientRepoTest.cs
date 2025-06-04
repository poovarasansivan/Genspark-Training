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
    public class PatinetRepositoryTest
    {
        private ClinicContext _context = null!;
        private PatinetRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new ClinicContext(options);
            _repository = new PatinetRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_ReturnsPatient_WhenPatientExists()
        {
            // Arrange
            var patient = new Patient
            {
                Id = 1,
                Name = "Poovarasan",
                Age = 35,
                Email = "poovarasan@gmail.com",
                Phone = "1234567890"
            };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.Get(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Poovarasan"));
            Assert.That(result.Email, Is.EqualTo("poovarasan@gmail.com"));
        }

        [Test]
        public void Get_ThrowsException_WhenPatientDoesNotExist()
        {
            // Arrange
            var nonExistingId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Get(nonExistingId));
            Assert.That(ex?.Message, Is.EqualTo("No patient with teh given ID"));
        }

        [Test]
        public async Task GetAll_ReturnsAllPatients_WhenPatientsExist()
        {
            // Arrange
            var patients = new List<Patient>
            {
                new Patient { Id = 1, Name = "Poovarasan", Age = 35, Email = "poovarasan@gmail.com", Phone = "1234567890" },
                new Patient { Id = 2, Name = "Jane Smith", Age = 28, Email = "jane.smith@example.com", Phone = "0987654321" }
            };
            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            // Act
            var results = await _repository.GetAll();

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAll_ThrowsException_WhenNoPatientsExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.That(ex?.Message, Is.EqualTo("No Patients in the database"));
        }
    }
}
