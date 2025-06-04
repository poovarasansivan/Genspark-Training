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
    public class DoctorSpecialityRepositoryTest
    {
        private ClinicContext _context = null!;
        private DoctorSpecialityRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new ClinicContext(options);
            _repository = new DoctorSpecialityRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_ReturnsDoctorSpeciality_WhenExists()
        {
            // Arrange
            var doctorSpeciality = new DoctorSpeciality
            {
                SerialNumber = 1,
                DoctorId = 101,
                SpecialityId = 201
            };
            await _context.DoctorSpecialities.AddAsync(doctorSpeciality);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.Get(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SerialNumber, Is.EqualTo(1));
            Assert.That(result.DoctorId, Is.EqualTo(101));
            Assert.That(result.SpecialityId, Is.EqualTo(201));
        }

        [Test]
        public void Get_ThrowsException_WhenDoctorSpecialityDoesNotExist()
        {
            // Arrange
            var nonExistingId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.Get(nonExistingId));
            Assert.That(ex?.Message, Is.EqualTo("No doctor specialities with the given ID"));
        }

        [Test]
        public async Task GetAll_ReturnsAllDoctorSpecialities_WhenExist()
        {
            // Arrange
            var specialities = new List<DoctorSpeciality>
            {
                new DoctorSpeciality { SerialNumber = 1, DoctorId = 101, SpecialityId = 201 },
                new DoctorSpeciality { SerialNumber = 2, DoctorId = 102, SpecialityId = 202 }
            };
            await _context.DoctorSpecialities.AddRangeAsync(specialities);
            await _context.SaveChangesAsync();

            // Act
            var results = await _repository.GetAll();

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAll_ThrowsException_WhenNoDoctorSpecialitiesExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetAll());
            Assert.That(ex?.Message, Is.EqualTo("No doctor speciality in the database"));
        }
    }
}
