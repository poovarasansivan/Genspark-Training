using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Services;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessTracking.Tests.Services
{
    [TestFixture]
    public class ProgressServiceTests
    {
        private FitnessContext _context;
        private ProgressService _service;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);
            _httpContext = new DefaultHttpContext();
            var accessor = new HttpContextAccessor { HttpContext = _httpContext };

            var repo = new Repositories.ProgressRepository(_context);
            _service = new ProgressService(_context, repo, accessor);
        }

        [Test]
        public async Task AddProgressAsync_ShouldAddSuccessfully()
        {
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User" };
            var workout = new WorkOutPlanModel { Id = Guid.NewGuid(), Name = "Test Plan" };
            await _context.Users.AddAsync(user);
            await _context.WorkOutPlans.AddAsync(workout);
            await _context.SaveChangesAsync();

            var dto = new ProgressAddRequestDto
            {
                UserId = user.Id,
                WorkOutPlanId = workout.Id,
                Date = DateTime.Today,
                Weight = 70,
                BodyFatPercentage = 20,
                MuscleMass = 50,
                WaterPercentage = 60,
                Notes = "Test progress"
            };

            await _service.AddProgressAsync(dto);

            Assert.That(_context.ProgressUpdates.First().UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public void AddProgressAsync_ShouldThrow_WhenProgressIsNull()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.AddProgressAsync(null));
            Assert.That(ex.Message, Is.EqualTo("Progress data cannot be null"));
        }

        [Test]
        public void AddProgressAsync_ShouldThrow_WhenWorkOutPlanIdIsEmpty()
        {
            var dto = new ProgressAddRequestDto { WorkOutPlanId = Guid.Empty };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.AddProgressAsync(dto));
            Assert.That(ex.Message, Is.EqualTo("WorkOut Plan ID cannot be empty or null"));
        }

        [Test]
        public async Task GetProgressByIdAsync_ShouldReturnProgress()
        {
            var progress = await SeedProgressAsync();

            var result = await _service.GetProgressByIdAsync(progress.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(progress.Id));
        }

        [Test]
        public void GetProgressByIdAsync_ShouldThrow_WhenIdIsEmpty()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetProgressByIdAsync(Guid.Empty));
            Assert.That(ex.Message, Is.EqualTo("Progress ID cannot be empty or null"));
        }

        [Test]
        public void GetProgressByIdAsync_ShouldThrow_WhenProgressNotFound()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetProgressByIdAsync(Guid.NewGuid()));
            Assert.That(ex.Message, Is.EqualTo("Progress not found"));
        }

        [Test]
        public async Task GetAllProgressAsync_ShouldReturnList()
        {
            await SeedProgressAsync();

            var result = await _service.GetAllProgressAsync();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetAllProgressAsync_ShouldThrow_WhenNoData()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetAllProgressAsync());
            Assert.That(ex.Message, Is.EqualTo("No progress updates found"));
        }

        [Test]
        public async Task GetProgressByUserIdAndWorkOutPlanIdAsync_ShouldReturnProgress()
        {
            var progress = await SeedProgressAsync();

            var result = await _service.GetProgressByUserIdAndWorkOutPlanIdAsync(progress.UserId, progress.WorkOutPlanId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetProgressByUserIdAndWorkOutPlanIdAsync_ShouldThrow_WhenIdsInvalid()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetProgressByUserIdAndWorkOutPlanIdAsync(Guid.Empty, Guid.NewGuid()));
            Assert.That(ex.Message, Is.EqualTo("User ID and WorkOut Plan ID cannot be empty or null"));
        }

        [Test]
        public void GetProgressByUserIdAndWorkOutPlanIdAsync_ShouldThrow_WhenProgressNotFound()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetProgressByUserIdAndWorkOutPlanIdAsync(Guid.NewGuid(), Guid.NewGuid()));
            Assert.That(ex.Message, Is.EqualTo("Progress not found for the specified user and workout plan"));
        }

        [Test]
        public async Task GetUserProgressByWorkOutPlanIdAsync_ShouldReturnList()
        {
            var progress = await SeedProgressAsync();

            var result = await _service.GetUserProgressByWorkOutPlanIdAsync(progress.WorkOutPlanId);

            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetUserProgressByWorkOutPlanIdAsync_ShouldThrow_WhenIdEmpty()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetUserProgressByWorkOutPlanIdAsync(Guid.Empty));
            Assert.That(ex.Message, Is.EqualTo("WorkOut Plan ID cannot be empty or null"));
        }

        [Test]
        public void GetUserProgressByWorkOutPlanIdAsync_ShouldThrow_WhenNotFound()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetUserProgressByWorkOutPlanIdAsync(Guid.NewGuid()));
            Assert.That(ex.Message, Is.EqualTo("No progress updates found for the specified workout plan"));
        }

        private async Task<ProgressModel> SeedProgressAsync()
        {
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Seed User" };
            var workout = new WorkOutPlanModel { Id = Guid.NewGuid(), Name = "Seed Plan" };
            await _context.Users.AddAsync(user);
            await _context.WorkOutPlans.AddAsync(workout);

            var progress = new ProgressModel
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                WorkOutPlanId = workout.Id,
                Date = DateTime.Today,
                Weight = 75,
                BodyFatPercentage = 22,
                MuscleMass = 45,
                WaterPercentage = 58,
                Notes = "Seeded progress",
                CreatedAt = DateTime.UtcNow
            };
            await _context.ProgressUpdates.AddAsync(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
