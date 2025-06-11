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
    public class ProgressRepositoryTests
    {
        private FitnessContext _context;
        private FitnessTracking.Repositories.ProgressRepository _repository;
        private Guid _progressId;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);
            _repository = new ProgressRepository(_context);

            // Seed one record
            _progressId = Guid.NewGuid();
            _context.ProgressUpdates.Add(new ProgressModel
            {
                Id = _progressId,
                UserId = Guid.NewGuid(),
                Weight = 70,
                Date = DateTime.UtcNow
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        // --------- GetAllAsync ---------
        [Test]
        public async Task GetAllAsync_ReturnsAllProgress()
        {
            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task GetAllAsync_EmptyTable_ReturnsEmptyList()
        {
            _context.ProgressUpdates.RemoveRange(_context.ProgressUpdates);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _repository.GetAllAsync());
        }

        // --------- GetByIdAsync ---------
        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsProgress()
        {
            var result = await _repository.GetByIdAsync(_progressId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(_progressId));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByIdAsync_EmptyId_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _repository.GetByIdAsync(Guid.Empty));
            Assert.That(ex!.Message, Does.Contain("Progress ID cannot be empty or null"));
        }
    }
}
