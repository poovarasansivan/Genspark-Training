using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Misc;
using FitnessTracking.Repositories;
using FitnessTracking.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FitnessTracking.Tests.Services
{
    [TestFixture]
    public class WorkOutLogServiceTests
    {
        private FitnessContext _context;
        private WorkOutLogService _service;
        private WorkOutLogRepository _repository;

        private Guid _userId = Guid.Parse("25e46d54-64a1-4366-9f4e-1c462f4c84fc");
        private Guid _planId = Guid.Parse("c994b86e-4504-4e63-bb26-68381e142f2c");
        private Guid _logId = Guid.Parse("062f9a04-96bb-48f6-a24b-1ff19a563e9f");

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);
            _repository = new WorkOutLogRepository(_context);
            _service = new WorkOutLogService(_repository, _context);

            // Seed user and plan
            _context.Users.Add(new UserModel { Id = _userId, Name = "Test User" });
            _context.WorkOutPlans.Add(new WorkOutPlanModel { Id = _planId, Name = "Plan A", Description = "Sample Plan" });
            _context.SaveChanges();
        }

        [Test]
        public async Task AddWorkOutLogAsync_Valid_AddsSuccessfully()
        {
            var dto = new WorkOutLogAddRequestDto
            {
                UserId = _userId,
                WorkOutPlanId = _planId,
                Type = "Cardio",
                Date = DateTime.UtcNow,
                Duration = TimeSpan.FromMinutes(30),
                CaloriesBurned = 250,
                Notes = "Morning workout"
            };

            await _service.AddWorkOutLogAsync(dto);
            Assert.That(1, Is.EqualTo(_context.Workouts.Count()));
        }

        [Test]
        public void AddWorkOutLogAsync_NullDto_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.AddWorkOutLogAsync(null));
            Assert.That(ex.Message, Does.Contain("WorkOut Log data cannot be null"));
        }

        [Test]
        public async Task DeleteWorkOutLogAsync_ValidId_DeletesSuccessfully()
        {
            var log = new WorkoutModel
            {
                Id = _logId,
                UserId = _userId,
                WorkOutPlanId = _planId,
                Type = "Strength",
                Date = DateTime.UtcNow,
                Duration = TimeSpan.FromMinutes(40),
                CaloriesBurned = 300,
                Notes = "Evening"
            };
            _context.Workouts.Add(log);
            await _context.SaveChangesAsync();

            await _service.DeleteWorkOutLogAsync(_logId);
            Assert.That(_context.Workouts, Is.Empty);
        }

        [Test]
        public void DeleteWorkOutLogAsync_NonExistingId_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteWorkOutLogAsync(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("entity"));
        }


        [Test]
        public async Task GetAllWorkOutLogsAsync_Valid_ReturnsList()
        {
            await AddSampleLog();
            var result = await _service.GetAllWorkOutLogsAsync();
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetAllWorkOutLogsAsync_NoLogs_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetAllWorkOutLogsAsync());
            Assert.That(ex.Message, Does.Contain("No workout logs found"));
        }

        [Test]
        public void GetAllWorkOutLogsAsync_DbNull_Throws()
        {
            _context.Workouts = null;
            Assert.ThrowsAsync<NullReferenceException>(() => _service.GetAllWorkOutLogsAsync());
        }

        [Test]
        public async Task GetWorkOutLogByIdAsync_Valid_ReturnsLog()
        {
            await AddSampleLog();
            var log = _context.Workouts.First();
            var result = await _service.GetWorkOutLogByIdAsync(log.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetWorkOutLogByIdAsync_EmptyId_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetWorkOutLogByIdAsync(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("WorkOut Log ID cannot be empty"));
        }

        [Test]
        public void GetWorkOutLogByIdAsync_NotFound_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetWorkOutLogByIdAsync(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("Workout log not found"));
        }

        [Test]
        public async Task GetUserWorkOutLogsAsync_Valid_ReturnsUserLogs()
        {
            await AddSampleLog();
            var logs = await _service.GetUserWorkOutLogsAsync(_userId);
            Assert.That(logs, Is.Not.Empty);
        }

        [Test]
        public void GetUserWorkOutLogsAsync_EmptyId_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetUserWorkOutLogsAsync(Guid.Empty));
            Assert.That(ex.Message, Does.Contain("User ID cannot be empty"));
        }

        [Test]
        public void GetUserWorkOutLogsAsync_NotFound_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetUserWorkOutLogsAsync(Guid.NewGuid()));
            Assert.That(ex.Message, Does.Contain("No workout logs found"));
        }

        [Test]
        public async Task GetPaginatedWorkOutLogsAsync_Valid_ReturnsPaginated()
        {
            await AddSampleLog();
            var filter = new WorkOutLogFilterDto { PageNumber = 1, PageSize = 10 };
            var result = await _service.GetPaginatedWorkOutLogsAsync(filter);
            Assert.That(result.Data, Is.Not.Empty);
        }

        [Test]
        public void GetPaginatedWorkOutLogsAsync_NullFilter_Throws()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetPaginatedWorkOutLogsAsync(null));
            Assert.That(ex.Message, Does.Contain("Filter data cannot be null"));
        }

        [Test]
        public void GetPaginatedWorkOutLogsAsync_InvalidPagination_Throws()
        {
            var filter = new WorkOutLogFilterDto { PageNumber = 0, PageSize = 0 };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetPaginatedWorkOutLogsAsync(filter));
            Assert.That(ex.Message, Does.Contain("greater than zero"));
        }

        private async Task AddSampleLog()
        {
            var log = new WorkoutModel
            {
                UserId = _userId,
                WorkOutPlanId = _planId,
                Type = "Strength",
                Date = DateTime.UtcNow,
                Duration = TimeSpan.FromMinutes(30),
                CaloriesBurned = 200,
                Notes = "Sample log"
            };
            _context.Workouts.Add(log);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
