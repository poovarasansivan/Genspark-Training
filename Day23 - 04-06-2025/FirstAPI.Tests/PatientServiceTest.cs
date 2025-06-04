using NUnit.Framework;
using Moq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Services;
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Test
{
    public class PatientServiceTest
    {
        [Test]
        public async Task TestAddPatient_Success()
        {
            // Arrange
            var patientDto = new PatientAddRequestDto
            {
                Id = 0,
                Name = "Kavin",
                Age = 30,
                Email = "kavin@gmail.com",
                Phone = "1234567890",
                Password = "password123"
            };

            var mapperMock = new Mock<IMapper>();
            var userRepositoryMock = new Mock<IRepository<string, User>>();
            var encryptionServiceMock = new Mock<IEncryptionService>();
            var patientRepositoryMock = new Mock<IRepository<int, Patient>>();

            mapperMock.Setup(m => m.Map<PatientAddRequestDto, User>(It.IsAny<PatientAddRequestDto>()))
                      .Returns((PatientAddRequestDto dto) => new User
                      {
                          Username = dto.Email
                      });

            mapperMock.Setup(m => m.Map<PatientAddRequestDto, Patient>(It.IsAny<PatientAddRequestDto>()))
                      .Returns((PatientAddRequestDto dto) => new Patient
                      {
                          Name = dto.Name,
                          Age = dto.Age,
                          Phone = dto.Phone
                      });

            encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                                 .ReturnsAsync(new EncryptModel
                                 {
                                     EncryptedData = Encoding.UTF8.GetBytes("encrypted"),
                                     HashKey = Encoding.UTF8.GetBytes("hash")
                                 });

            userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                              .ReturnsAsync((User u) =>
                              {
                                  u.Role = "Patient";
                                  return u;
                              });

            patientRepositoryMock.Setup(r => r.Add(It.IsAny<Patient>()))
                                 .ReturnsAsync((Patient p) =>
                                 {
                                     p.Id = 1;
                                     return p;
                                 });

            var patientService = new PatientService(
                mapperMock.Object,
                userRepositoryMock.Object,
                encryptionServiceMock.Object,
                patientRepositoryMock.Object
            );

            // Act
            var result = await patientService.AddPatient(patientDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Kavin"));
            Assert.That(result.Phone, Is.EqualTo("1234567890"));

            encryptionServiceMock.Verify(e => e.EncryptData(It.IsAny<EncryptModel>()), Times.Once);
            userRepositoryMock.Verify(r => r.Add(It.Is<User>(u => u.Username == patientDto.Email && u.Role == "Patient")), Times.Once);
            patientRepositoryMock.Verify(r => r.Add(It.IsAny<Patient>()), Times.Once);
        }
    }
}
