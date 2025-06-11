using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessTracking.Repository
{
    [TestFixture]
    public class WorkOutPlanRepositoryTests
    {
        private FitnessContext _context;
        private WorkOutPlanRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);

            var plan = new WorkOutPlanModel
            {
                Id = Guid.NewGuid(),
                Name = "Beginner Plan",
                Description = "A basic plan",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkOutPlans.Add(plan);
            _context.SaveChanges();

            _repository = new WorkOutPlanRepository(_context);
        }

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsPlan()
        {
            var existing = _context.WorkOutPlans.First();
            var result = await _repository.GetByIdAsync(existing.Id);
            Assert.That(result,Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(existing.Id));
        }

        [Test]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetByIdAsync_ThrowsExceptionIfContextDisposed()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var tempContext = new FitnessContext(options);
            var repo = new WorkOutPlanRepository(tempContext);
            tempContext.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(() => repo.GetByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task GetAllAsync_ValidCall_ReturnsPlans()
        {
            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
        }

        [Test]
        public async Task GetAllAsync_EmptyDb_ReturnsEmptyList()
        {
            _context.WorkOutPlans.RemoveRange(_context.WorkOutPlans);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetAllAsync_ThrowsExceptionIfContextDisposed()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var tempContext = new FitnessContext(options);
            var repo = new WorkOutPlanRepository(tempContext);
            tempContext.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(() => repo.GetAllAsync());
        }

        [Test]
        public async Task AddAsync_ValidEntity_AddsPlan()
        {
            var newPlan = new WorkOutPlanModel
            {
                Id = Guid.NewGuid(),
                Name = "New Plan",
                Description = "New Desc",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newPlan);
            var fetched = await _repository.GetByIdAsync(newPlan.Id);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched.Name, Is.EqualTo("New Plan"));
        }

        [Test]
        public void AddAsync_InvalidEntity_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _repository.AddAsync(null));
        }

        [Test]
        public void AddAsync_ThrowsExceptionIfContextDisposed()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var tempContext = new FitnessContext(options);
            var repo = new WorkOutPlanRepository(tempContext);
            tempContext.Dispose();

            var plan = new WorkOutPlanModel { Id = Guid.NewGuid(), Name = "Disposed Plan" };
            Assert.ThrowsAsync<ObjectDisposedException>(() => repo.AddAsync(plan));
        }

        [TearDown]
        public void TearDown()
        {
            if (_context?.Database != null)
            {
                try { _context.Database.EnsureDeleted(); } catch { }
            }

            try { _context?.Dispose(); } catch { }
        }
    }
}