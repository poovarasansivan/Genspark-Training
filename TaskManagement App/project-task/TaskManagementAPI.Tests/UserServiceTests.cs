using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementAPI.DTOs.Users;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Mapper;
using TaskManagementAPI.Models;
using TaskManagementAPI.Enums;
using TaskManagementAPI.Services;
using TaskManagementAPI.CustomExceptions;
using TaskManagementAPI.ApiResponses;

namespace TaskManagementAPI.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private UserMapper _userMapper;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _userMapper = new UserMapper();
            _userService = new UserService(_userRepoMock.Object, _userMapper);
        }

        [Test]
        public async Task GetAllAsync_ReturnsPagedResponse()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "John",
                    Email = "john@example.com",
                    Role = UserRole.Manager,
                    PasswordHash = "hashed"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane",
                    Email = "jane@example.com",
                    Role = UserRole.TeamMember,
                    PasswordHash = "hashed"
                }
            };

            _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(users);

            var result = await _userService.GetAllAsync(page: 1, pageSize: 1);

            Assert.AreEqual(1, result.Data.Count());
        }

        [Test]
        public async Task GetByIdAsync_ExistingUser_ReturnsUser()
        {
            var id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                FullName = "Alice",
                Email = "alice@example.com",
                Role = UserRole.TeamMember,
                PasswordHash = "hashed"
            };

            _userRepoMock.Setup(x => x.Get(id)).ReturnsAsync(user);

            var result = await _userService.GetByIdAsync(id);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data); 
            Assert.AreEqual(user.Email, result.Data!.Email);
        }

        [Test]
        public async Task GetByIdAsync_NonExistingUser_ReturnsError()
        {
            var id = Guid.NewGuid();
            _userRepoMock.Setup(x => x.Get(id)).ReturnsAsync((User)null!);

            var result = await _userService.GetByIdAsync(id);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task CreateAsync_ValidUser_ReturnsSuccessResponse()
        {
            var dto = new UserRequestDto
            {
                FullName = "New User",
                Email = "newuser@example.com",
                Password = "pass123",
                Role = UserRole.Manager
            };

            _userRepoMock.Setup(x => x.GetByEmail(dto.Email)).ReturnsAsync((User)null!);
            _userRepoMock.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync((User u) => u);

            var result = await _userService.CreateAsync(dto, Guid.NewGuid());

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(dto.Email, result.Data!.Email);
        }

        [Test]
        public void CreateAsync_EmailAlreadyExists_ThrowsConflictException()
        {
            var dto = new UserRequestDto
            {
                FullName = "Duplicate",
                Email = "exists@example.com",
                Password = "password",
                Role = UserRole.Manager
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Dup",
                Email = dto.Email,
                Role = UserRole.Manager,
                PasswordHash = "hashed"
            };

            _userRepoMock.Setup(x => x.GetByEmail(dto.Email)).ReturnsAsync(existingUser);

            Assert.ThrowsAsync<ConflictException>(() => _userService.CreateAsync(dto, Guid.NewGuid()));
        }

        [Test]
        public async Task UpdateAsync_ValidUser_UpdatesSuccessfully()
        {
            var id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                FullName = "Old Name",
                Email = "update@example.com",
                Role = UserRole.TeamMember,
                PasswordHash = "oldhash"
            };

            var updateDto = new UserUpdateDto
            {
                FullName = "Updated Name",
                Password = "newpass",
                Role = UserRole.Manager
            };

            _userRepoMock.Setup(x => x.Get(id)).ReturnsAsync(user);

            _userRepoMock.Setup(x => x.Update(id, It.IsAny<User>()))
                        .ReturnsAsync((Guid k, User u) => u);

            var result = await _userService.UpdateAsync(id, updateDto, Guid.NewGuid());

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Updated Name", result.Data.FullName);
        }

        [Test]
        public void UpdateAsync_UserNotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _userRepoMock.Setup(x => x.Get(id)).ReturnsAsync((User)null!);

            var updateDto = new UserUpdateDto { FullName = "Update" };

            Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateAsync(id, updateDto, Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteAsync_ValidUser_DeletesSuccessfully()
        {
            var id = Guid.NewGuid();
            var user = new User
            {
                Id = id,
                FullName = "ToDelete",
                Email = "delete@example.com",
                PasswordHash = "hashed",
                Role = UserRole.TeamMember
            };

            _userRepoMock.Setup(x => x.Get(id)).ReturnsAsync(user);

           
            _userRepoMock.Setup(x => x.Update(id, It.IsAny<User>()))
                        .ReturnsAsync((Guid k, User u) => u);

            var result = await _userService.DeleteAsync(id);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("User deleted successfully", result.Data);
        }


        [Test]
        public void DeleteAsync_UserNotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _userRepoMock.Setup(x => x.Get(id)).ReturnsAsync((User)null!);

            Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteAsync(id));
        }
    }
}
