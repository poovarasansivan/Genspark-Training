using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessTracking.Repository
{
    [TestFixture]
    public class UserPlanRepositoryTests
    {
        private FitnessContext _context;
        private UserPlanRepository _repository;
        private Guid _validPlanId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);
            _repository = new UserPlanRepository(_context);

            _validPlanId = Guid.NewGuid();

            _context.UserWorkOutPlans.Add(new UserWorkOutPlanModel
            {
                Id = _validPlanId,
                UserId = Guid.NewGuid(),
                WorkOutPlanId = Guid.NewGuid(),
            });

            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsPlan()
        {
            var result = await _repository.GetByIdAsync(_validPlanId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(_validPlanId));
        }

        [Test]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByIdAsync_EmptyId_DoesNotThrowButReturnsNull()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var result = await _repository.GetByIdAsync(Guid.Empty);
                Assert.That(result, Is.Null);
            });
        }


        [Test]
        public async Task GetAllAsync_WithData_ReturnsAll()
        {
            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task GetAllAsync_EmptyTable_ReturnsEmptyList()
        {
            _context.UserWorkOutPlans.RemoveRange(_context.UserWorkOutPlans);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAllAsync_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _repository.GetAllAsync());
        }
    }
}
