using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Repositories;
using FitnessTracking.Services;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FitnessTracking.Tests.Services
{
    public class UserPlanServiceTests
    {
        private FitnessContext _context;
        private UserPlanService _service;
        private Guid _userId = Guid.NewGuid();
        private Guid _workoutPlanId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);
            _context.Users.Add(new UserModel { Id = _userId, Name = "TestUser", Email = "test@example.com", Role = "Client", IsActive = true });
            _context.WorkOutPlans.Add(new WorkOutPlanModel { Id = _workoutPlanId, Name = "Plan A", Description = "Test Desc" });
            _context.SaveChanges();

            var userPlanRepo = new UserPlanRepository(_context);
            var workOutPlanRepo = new WorkOutPlanRepository(_context);

            _service = new UserPlanService(userPlanRepo, workOutPlanRepo, _context);
        }

        [Test]
        public async Task AddUserPlanAsync_ValidData_ShouldAddPlan()
        {
            var dto = new UserPlanAddRequestDto { UserId = _userId, WorkOutPlanId = _workoutPlanId };
            await _service.AddUserPlanAsync(dto);

            Assert.That(_context.UserWorkOutPlans.Any(up => up.UserId == _userId && up.WorkOutPlanId == _workoutPlanId), Is.True);
        }

        [Test]
        public void AddUserPlanAsync_InvalidData_ShouldThrow()
        {
            var dto = new UserPlanAddRequestDto { UserId = Guid.Empty, WorkOutPlanId = Guid.Empty };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.AddUserPlanAsync(dto));
            Assert.That(ex.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void AddUserPlanAsync_NullDto_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.AddUserPlanAsync(null));
            Assert.That(ex.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task DeleteUserPlanAsync_ValidId_ShouldDelete()
        {
            var userPlan = new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId };
            _context.UserWorkOutPlans.Add(userPlan);
            await _context.SaveChangesAsync();

            await _service.DeleteUserPlanAsync(userPlan.Id);

            Assert.That(await _context.UserWorkOutPlans.FindAsync(userPlan.Id), Is.Null);
        }

        [Test]
        public void DeleteUserPlanAsync_InvalidId_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.DeleteUserPlanAsync(Guid.NewGuid()));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void DeleteUserPlanAsync_EmptyId_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.DeleteUserPlanAsync(Guid.Empty));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllUserPlansAsync_Valid_ShouldReturnList()
        {
            _context.UserWorkOutPlans.Add(new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllUserPlansAsync();

            Assert.That(result.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void GetAllUserPlansAsync_Empty_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetAllUserPlansAsync());
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllUserPlansAsync_NullName_FallbackHandled()
        {
            _context.UserWorkOutPlans.Add(new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllUserPlansAsync();
            Assert.That(result.First().UserName, Is.EqualTo("TestUser"));
        }

        [Test]
        public async Task GetUserPlanByIdAsync_ValidId_ShouldReturnPlan()
        {
            var plan = new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId };
            _context.UserWorkOutPlans.Add(plan);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserPlanByIdAsync(plan.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetUserPlanByIdAsync_NonExisting_ShouldReturnNull()
        {
            var result = await _service.GetUserPlanByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserPlanByIdAsync_EmptyId_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetUserPlanByIdAsync(Guid.Empty));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetUserPlansByUserIdAsync_ValidId_ShouldReturnList()
        {
            _context.UserWorkOutPlans.Add(new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId });
            await _context.SaveChangesAsync();

            var result = await _service.GetUserPlansByUserIdAsync(_userId);
            Assert.That(result.Count(), Is.GreaterThan(0));
        }

        [Test]
        public async Task GetUserPlansByUserIdAsync_NoPlans_ShouldReturnEmpty()
        {
            var result = await _service.GetUserPlansByUserIdAsync(Guid.NewGuid());
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetUserPlansByUserIdAsync_EmptyId_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetUserPlansByUserIdAsync(Guid.Empty));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateUserPlanAsync_Valid_ShouldUpdate()
        {
            var plan = new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId };
            _context.UserWorkOutPlans.Add(plan);
            await _context.SaveChangesAsync();

            var dto = new UserPlanUpdateDto { IsCompleted = "Yes", UpdatedAt = DateTime.UtcNow };
            await _service.UpdateUserPlanAsync(plan.Id, dto);

            var updated = await _context.UserWorkOutPlans.FindAsync(plan.Id);
            Assert.That(updated.IsCompleted, Is.EqualTo("Yes"));
        }

        [Test]
        public void UpdateUserPlanAsync_InvalidId_ShouldThrow()
        {
            var dto = new UserPlanUpdateDto { IsCompleted = "Yes" };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.UpdateUserPlanAsync(Guid.NewGuid(), dto));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void UpdateUserPlanAsync_EmptyId_ShouldThrow()
        {
            var dto = new UserPlanUpdateDto { IsCompleted = "Yes" };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.UpdateUserPlanAsync(Guid.Empty, dto));
            Assert.That(ex.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetPaginatedUserPlansAsync_Valid_ShouldReturnPaged()
        {
            _context.UserWorkOutPlans.Add(new UserWorkOutPlanModel { UserId = _userId, WorkOutPlanId = _workoutPlanId });
            await _context.SaveChangesAsync();

            var filter = new UserWorkOutPlanFilterDto { PageNumber = 1, PageSize = 10 };
            var result = await _service.GetPaginatedUserPlansAsync(filter);

            Assert.That(result.Data.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void GetPaginatedUserPlansAsync_InvalidPage_ShouldThrow()
        {
            var filter = new UserWorkOutPlanFilterDto { PageNumber = 0, PageSize = 0 };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetPaginatedUserPlansAsync(filter));
            Assert.That(ex.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void GetPaginatedUserPlansAsync_NullFilter_ShouldThrow()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _service.GetPaginatedUserPlansAsync(null));
            Assert.That(ex.StatusCode, Is.EqualTo(400));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
