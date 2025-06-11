using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessTracking.Contexts;
using FitnessTracking.Interfaces;
using FitnessTracking.Misc;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Repositories;
using FitnessTracking.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace FitnessTracking.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private FitnessContext _context;
        private UserService _userService;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private UserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FitnessContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitnessContext(options);
            _context.Database.EnsureCreated();

            _encryptionServiceMock = new Mock<IEncryptionService>();
            _userRepository = new UserRepository(_context);
            _userService = new UserService(_userRepository, _encryptionServiceMock.Object, _context);
        }


        [Test]
        public async Task AddUserAsync_ValidUser_AddsUser()
        {
            var dto = new UserRegisterDto { Name = "Test", Email = "test@gmail.com", Password = "Test@123" };
            _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(new EncryptModel { EncryptedData = "Encrypted" });

            await _userService.AddUserAsync(dto);

            Assert.That(1, Is.EqualTo(_context.Users.Count()));
        }

        [Test]
        public void DeleteUserAsync_InvalidId_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _userService.DeleteUserAsync(Guid.Empty));
            Assert.That(ex.Message, Is.EqualTo("User ID could not be empty or null"));
        }


        [Test]
        public async Task GetUserByIdAsync_ValidId_ReturnsUser()
        {
            var user = new UserModel { Name = "Test", Email = "test@gmail.com", Password = "pass" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _userService.GetUserByIdAsync(user.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(user.Name, Is.EqualTo(result.Name));
        }

        [Test]
        public async Task GetUserByIdAsync_InvalidId_ReturnsNull()
        {
            var result = await _userService.GetUserByIdAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdatePasswordAsync_UserNotFound_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _userService.UpdatePasswordAsync(Guid.NewGuid(), new UpdatePasswordDto { Password = "newpass" }));
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task UpdateUserAsync_ValidUpdate_UpdatesUser()
        {
            var user = new UserModel { Name = "Old", Email = "old@gmail.com", Password = "pass" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new UpdateUserDto { Name = "New" };
            await _userService.UpdateUserAsync(user.Id, dto);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.That("New", Is.EqualTo(updatedUser.Name));
        }

        [Test]
        public void UpdateUserAsync_UserNotFound_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _userService.UpdateUserAsync(Guid.NewGuid(), new UpdateUserDto { Name = "Test" }));
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            _context.Users.Add(new UserModel { Name = "User1", Email = "user1@gmail.com", Password = "pass" });
            _context.Users.Add(new UserModel { Name = "User2", Email = "user2@gmail.com", Password = "pass" });
            await _context.SaveChangesAsync();

            var users = await _userService.GetAllUsersAsync();

            Assert.That(2, Is.EqualTo(users.Count()));

        }

        [Test]
        public async Task GetPaginatedAllUsersAsync_ValidPagination_ReturnsPaginatedUsers()
        {
            for (int i = 1; i <= 5; i++)
            {
                _context.Users.Add(new UserModel { Name = $"User{i}", Email = $"user{i}@gmail.com", Password = "pass" });
            }
            await _context.SaveChangesAsync();

            var pagination = new PaginationParameterDto { PageNumber = 1, PageSize = 3, SortDirection = "asc" };
            var result = await _userService.GetPaginatedAllUsersAsync(pagination);
            Assert.That(result.Data.Count, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(5));
        }

        [Test]
        public void GetPaginatedAllUsersAsync_InvalidPagination_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<CustomeExceptionHandler>(() => _userService.GetPaginatedAllUsersAsync(null));
            Assert.That(ex.Message, Is.EqualTo("Pagination parameters could not be empty or null"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
