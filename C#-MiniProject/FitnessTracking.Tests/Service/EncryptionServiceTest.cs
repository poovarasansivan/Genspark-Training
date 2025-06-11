using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using FitnessTracking.Services;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace FitnessTracking.Tests
{
    public class EncryptionServiceTests
    {
        private IEncryptionService _encryptionService;

        [SetUp]
        public void Setup()
        {
            _encryptionService = new EncryptionService();
        }

        [Test]
        public async Task EncryptData_ValidData_ReturnsEncryptedData()
        {
            var data = new EncryptModel { Data = "password123" };
            var result = await _encryptionService.EncryptData(data);
            Assert.That(!string.IsNullOrEmpty(result.EncryptedData));
            Assert.That(data.Data, Is.Not.EqualTo(result.EncryptedData));
        }

        [Test]
        public void EncryptData_NullData_ThrowsArgumentException()
        {
            var data = new EncryptModel { Data = null };
            Assert.ThrowsAsync<ArgumentException>(() => _encryptionService.EncryptData(data));
        }

        [Test]
        public void EncryptData_EmptyData_ThrowsArgumentException()
        {
            var data = new EncryptModel { Data = "" };
            Assert.ThrowsAsync<ArgumentException>(() => _encryptionService.EncryptData(data));
        }

        [Test]
        public async Task VerifyPassword_ValidPassword_ReturnsTrue()
        {
            var data = new EncryptModel { Data = "password123" };
            var encryptedData = await _encryptionService.EncryptData(data);
            var result = await _encryptionService.VerifyPassword("password123", encryptedData.EncryptedData);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task VerifyPassword_InvalidPassword_ReturnsFalse()
        {
            var data = new EncryptModel { Data = "password123" };
            var encryptedData = await _encryptionService.EncryptData(data);
            var result = await _encryptionService.VerifyPassword("wrongpassword", encryptedData.EncryptedData);
            Assert.That(result, Is.False);
        }
    }
}