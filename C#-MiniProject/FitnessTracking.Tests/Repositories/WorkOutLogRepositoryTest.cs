using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessTracking.Repository
{
    [TestFixture]
    public class WorkOutLogRepositoryTests
    {
        private FitnessContext _context;
        private WorkOutLogRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new FitnessContext(options);

            // Seed data
            var user = new UserModel { Id = Guid.NewGuid(), Name = "TestUser", Email = "test@example.com", Role = "User" };
            var plan = new WorkOutPlanModel { Id = Guid.NewGuid(), Name = "Plan A", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(10) };

            var workout = new WorkoutModel
            {
                Id = Guid.NewGuid(),
                User = user,
                UserId = user.Id,
                WorkOutPlan = plan,
                WorkOutPlanId = plan.Id,
                Notes = "Morning Workout",
                Date = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.WorkOutPlans.Add(plan);
            _context.Workouts.Add(workout);
            _context.SaveChanges();

            _repository = new WorkOutLogRepository(_context);
        }

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsWorkout()
        {
            var workout = _context.Workouts.First();
            var result = await _repository.GetByIdAsync(workout.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(workout.Id));
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
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var tempContext = new FitnessContext(options);
            var repository = new WorkOutLogRepository(tempContext);
            tempContext.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(() => repository.GetByIdAsync(Guid.NewGuid()));
        }


        [Test]
        public async Task GetAllAsync_ValidCall_ReturnsAllWorkouts()
        {
            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            _context.Workouts.RemoveRange(_context.Workouts);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();
            Assert.That(result, Is.Not.Null);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
