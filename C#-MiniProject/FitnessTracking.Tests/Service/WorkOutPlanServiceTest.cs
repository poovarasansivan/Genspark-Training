using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Services;
using FitnessTracking.Repositories;
using FitnessTracking.Contexts;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FitnessTracking.Tests.Service
{
    public class WorkOutPlanServiceTests
    {
        private FitnessContext _context;
        private WorkOutPlanService _service;
        private WorkOutPlanRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new FitnessContext(options);

            // Seed data
            _context.WorkOutPlans.Add(new WorkOutPlanModel
            {
                Id = Guid.NewGuid(),
                Name = "Plan A",
                Description = "Initial plan",
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();

            _repository = new WorkOutPlanRepository(_context);
            _service = new WorkOutPlanService(_repository, _context);
        }

        [Test]
        public async Task AddWorkOutPlanAsync_ValidData_ShouldAddPlan()
        {
            var dto = new WorkOutAddRequestDto
            {
                Name = "Test Plan",
                Description = "Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(3)
            };

            await _service.AddWorkOutPlanAsync(dto);
            Assert.That(_context.WorkOutPlans.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task AddWorkOutPlanAsync_NullFields_ShouldUseDefaults()
        {
            var dto = new WorkOutAddRequestDto
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(2)
            };
            await _service.AddWorkOutPlanAsync(dto);
            var added = await _context.WorkOutPlans.FirstOrDefaultAsync(p => p.Name == "Default Plan");
            Assert.That(added, Is.Not.Null);
        }

        [Test]
        public void DeleteWorkOutPlanAsync_InvalidId_ShouldThrowException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(async () => await _service.DeleteWorkOutPlanAsync(Guid.Empty));
            Assert.That(ex!.Message, Is.EqualTo("WorkOut Plan ID could not be empty or null"));
        }

        [Test]
        public async Task GetAllWorkOutPlansAsync_Valid_ShouldReturnList()
        {
            var result = await _service.GetAllWorkOutPlansAsync();
            Assert.That(result.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void GetAllWorkOutPlansAsync_NoData_ShouldThrowException()
        {
            _context.WorkOutPlans.RemoveRange(_context.WorkOutPlans);
            _context.SaveChanges();
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(async () => await _service.GetAllWorkOutPlansAsync());
            Assert.That(ex!.Message, Is.EqualTo("No workout plans found"));
        }

        [Test]
        public async Task GetWorkOutPlanByIdAsync_ValidId_ShouldReturnPlan()
        {
            var plan = _context.WorkOutPlans.First();
            var result = await _service.GetWorkOutPlanByIdAsync(plan.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetWorkOutPlanByIdAsync_InvalidId_ShouldReturnNull()
        {
            var result = await _service.GetWorkOutPlanByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetWorkOutPlanByIdAsync_EmptyId_ShouldThrowException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(async () => await _service.GetWorkOutPlanByIdAsync(Guid.Empty));
            Assert.That(ex!.Message, Is.EqualTo("WorkOut Plan ID could not be empty or null"));
        }

        [Test]
        public async Task GetFilteredWorkOutPlansAsync_ValidSearch_ShouldReturnFiltered()
        {
            var filter = new WorkOutPlanFilterDto
            {
                SearchTerm = "Plan",
                PageNumber = 1,
                PageSize = 5,
                SortBy = "name",
                SortDirection = "asc"
            };
            var result = await _service.GetFilteredWorkOutPlansAsync(filter);
            Assert.That(result.Data.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task UpdateWorkOutPlanAsync_Valid_ShouldUpdate()
        {
            var plan = _context.WorkOutPlans.First();
            var dto = new WorkOutPlanUpdateDto
            {
                StartDate = plan.StartDate,
                EndDate = plan.EndDate.AddDays(2)
            };
            await _service.UpdateWorkOutPlanAsync(plan.Id, dto);
            var updated = await _context.WorkOutPlans.FindAsync(plan.Id);
            Assert.That(updated!.EndDate, Is.EqualTo(dto.EndDate));
        }

        [Test]
        public void UpdateWorkOutPlanAsync_InvalidId_ShouldThrow()
        {
            var dto = new WorkOutPlanUpdateDto { StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1) };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(async () => await _service.UpdateWorkOutPlanAsync(Guid.Empty, dto));
            Assert.That(ex!.Message, Is.EqualTo("WorkOut Plan ID could not be empty or null"));
        }

        [Test]
        public void UpdateWorkOutPlanAsync_EndDateBeforeStart_ShouldThrow()
        {
            var plan = _context.WorkOutPlans.First();
            var dto = new WorkOutPlanUpdateDto
            {
                StartDate = plan.StartDate,
                EndDate = plan.StartDate.AddDays(-1)
            };
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(async () => await _service.UpdateWorkOutPlanAsync(plan.Id, dto));
            Assert.That(ex!.Message, Is.EqualTo("End date cannot be earlier than start date"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
