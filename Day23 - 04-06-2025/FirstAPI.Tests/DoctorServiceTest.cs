using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Interfaces;
using FirstAPI.Services;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Misc;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace FirstAPI.Test
{
    public class DoctorServiceTest
    {
        private ClinicContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                                .UseInMemoryDatabase("TestDb")
                                .Options;
            _context = new ClinicContext(options);
        }

        [Test]
        public async Task TestAddDoctor_Success()
        {
            // Arrange
            var doctorDto = new DoctorAddRequestDto
            {
                Name = "Dr. Poovarasan",
                Email = "poovarasan@gmail.com",
                Password = "pass123",
                Specialities = new List<SpecialityAddRequestDto>
                {
                    new SpecialityAddRequestDto { Name = "Neurology" }
                }
            };

            var doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
            var specialityRepositoryMock = new Mock<IRepository<int, Speciality>>();
            var doctorSpecialityRepositoryMock = new Mock<IRepository<int, DoctorSpeciality>>();
            var userRepositoryMock = new Mock<IRepository<string, User>>();
            var otherContextFunctionitiesMock = new Mock<IOtherContextFunctionities>();
            var encryptionServiceMock = new Mock<IEncryptionService>();
            var mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(m => m.Map<DoctorAddRequestDto, User>(It.IsAny<DoctorAddRequestDto>()))
                .Returns((DoctorAddRequestDto src) =>
                    new User
                    {
                        Username = src.Email,
                        Role = null,
                    });

            mapperMock
                .Setup(m => m.Map<User>(It.IsAny<DoctorAddRequestDto>()))
                .Returns((DoctorAddRequestDto src) =>
                    new User
                    {
                        Username = src.Email,
                        Role = null
                    });

            encryptionServiceMock
                .Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                .ReturnsAsync(new EncryptModel
                {
                    EncryptedData = Encoding.UTF8.GetBytes("encryptedPass"),
                    HashKey = Encoding.UTF8.GetBytes("hashKey")
                });

            userRepositoryMock
                .Setup(r => r.Add(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    user.Role = "Doctor";
                    return user;
                });

            doctorRepositoryMock
                .Setup(r => r.Add(It.IsAny<Doctor>()))
                .ReturnsAsync((Doctor doctor) =>
                {
                    doctor.Id = 100;
                    doctor.Name = doctorDto.Name;
                    return doctor;
                });

            specialityRepositoryMock
                .Setup(r => r.GetAll())
                .ReturnsAsync(new List<Speciality>());

            specialityRepositoryMock
                .Setup(r => r.Add(It.IsAny<Speciality>()))
                .ReturnsAsync((Speciality s) =>
                {
                    s.Id = 1;
                    return s;
                });

            doctorSpecialityRepositoryMock
                .Setup(r => r.Add(It.IsAny<DoctorSpeciality>()))
                .ReturnsAsync((DoctorSpeciality ds) => ds);

            var doctorService = new DoctorService(
                doctorRepositoryMock.Object,
                specialityRepositoryMock.Object,
                doctorSpecialityRepositoryMock.Object,
                userRepositoryMock.Object,
                otherContextFunctionitiesMock.Object,
                encryptionServiceMock.Object,
                mapperMock.Object
            );

            // Act
            var addedDoctor = await doctorService.AddDoctor(doctorDto);

            // Assert using NUnit Assert.That syntax
            Assert.That(addedDoctor, Is.Not.Null);
            Assert.That(addedDoctor.Id, Is.EqualTo(100), "Expected the mocked repository to return Id = 100");
            Assert.That(addedDoctor.Name, Is.EqualTo("Dr. Poovarasan"));

            userRepositoryMock.Verify(r => r.Add(
                It.Is<User>(u => u.Username == doctorDto.Email && u.Role == "Doctor")),
                Times.Once);

            encryptionServiceMock.Verify(e => e.EncryptData(
                It.IsAny<EncryptModel>()), Times.Once);

            doctorRepositoryMock.Verify(r => r.Add(
                It.IsAny<Doctor>()), Times.Once);

            specialityRepositoryMock.Verify(r => r.Add(
                It.IsAny<Speciality>()), Times.Once);

            doctorSpecialityRepositoryMock.Verify(r => r.Add(
                It.IsAny<DoctorSpeciality>()), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
