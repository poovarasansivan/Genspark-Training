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
    public class DoctorRepositoryTest
    {
        private ClinicContext _context = null!;
        private DoctorRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new ClinicContext(options);
            _repository = new DoctorRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_ReturnsDoctor_WhenDoctorExists()
        {
            // Arrange
            var doctor = new Doctor
            {
                Id = 1,
                Name = "Dr. Smith",
                Status = "Active",
                YearsOfExperience = 10.5f,
                Email = "dr.smith@example.com"
            };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.Get(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Dr. Smith"));
            Assert.That(result.Status, Is.EqualTo("Active"));
            Assert.That(result.YearsOfExperience, Is.EqualTo(10.5f));
            Assert.That(result.Email, Is.EqualTo("dr.smith@example.com"));
        }

        [Test]
        public void Get_ThrowsException_WhenDoctorDoesNotExist()
        {
            // Arrange
            var nonExistingId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Get(nonExistingId));
            Assert.That(ex?.Message, Is.EqualTo("No doctor with teh given ID"));
        }

        [Test]
        public async Task GetAll_ReturnsAllDoctors_WhenDoctorsExist()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    Name = "Dr. Smith",
                    Status = "Active",
                    YearsOfExperience = 10.5f,
                    Email = "dr.smith@example.com"
                },
                new Doctor
                {
                    Id = 2,
                    Name = "Dr. Poovarasan",
                    Status = "Inactive",
                    YearsOfExperience = 8.0f,
                    Email = "poovarasan@example.com"
                }
            };
            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var results = await _repository.GetAll();

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAll_ThrowsException_WhenNoDoctorsExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.That(ex?.Message, Is.EqualTo("No Doctor in the database"));
        }
    }
}
