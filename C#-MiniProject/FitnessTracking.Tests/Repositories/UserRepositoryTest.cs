using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracking.Contexts;
using FitnessTracking.Models;
using FitnessTracking.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FitnessTracking.Repository
{
    public class UserRepositoryTests
    {
        private FitnessContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: "UserRepositoryTestDb")
                .Options;
            _context = new FitnessContext(options);
            _repository = new UserRepository(_context);

            _context.Users.AddRange(new List<UserModel>
            {
                new UserModel { Id = Guid.NewGuid(), Name = "User One", Email = "one@test.com", Role = "User", IsActive = true },
                new UserModel { Id = Guid.NewGuid(), Name = "User Two", Email = "two@test.com", Role = "Coach", IsActive = true },
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddValidUser()
        {
            var user = new UserModel { Id = Guid.NewGuid(), Name = "New User", Email = "new@test.com", Role = "User", IsActive = true };
            await _repository.AddAsync(user);
            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That("New User", Is.EqualTo(result.Name));
        }

        [Test]
        public void AddAsync_ShouldThrowForNullEntity()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.AddAsync(null!));
        }

        [Test]
        public async Task AddAsync_ShouldSkipIfAlreadyTracked()
        {
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Tracked User", Email = "tracked@test.com", Role = "User", IsActive = true };
            _context.Users.Attach(user);
            await _repository.AddAsync(user);
            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteValidUser()
        {
            var user = _context.Users.First();
            await _repository.DeleteAsync(user.Id);
            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteAsync_ShouldThrowForInvalidId()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.DeleteAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteAsync_ShouldThrowIfUserNotFound()
        {
            var userId = Guid.NewGuid();
            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.DeleteAsync(userId));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            var user = _context.Users.First();
            user.Name = "Updated Name";
            await _repository.UpdateAsync(user);
            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That("Updated Name", Is.EqualTo(result.Name));
        }

        [Test]
        public void UpdateAsync_ShouldThrowForNull()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _repository.UpdateAsync(null!));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateMultipleFields()
        {
            var user = _context.Users.First();
            user.Name = "Multi Update";
            user.IsActive = false;
            await _repository.UpdateAsync(user);
            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That(result!.IsActive, Is.False);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnValidUser()
        {
            var user = _context.Users.First();
            var result = await _repository.GetByIdAsync(user.Id);
            Assert.That(result,Is.Not.Null);
            Assert.That(user.Id, Is.EqualTo(result!.Id));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNullForInvalidId()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var result = await _repository.GetAllAsync();
            Assert.That(2, Is.EqualTo(result.Count()));
        }
    }

    public class UserRepository : Repository<UserModel>
    {
        public UserRepository(FitnessContext context) : base(context) { }

        public override async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public override async Task<UserModel> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
