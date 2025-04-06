using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Schedule;
using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.BL.Services.Implementations;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IScheduleRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IStaffRepository;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace PrisonManagementSystem.Tests.Services
{
    public class StaffServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IStaffReadRepository> _mockStaffReadRepository;
        private readonly Mock<IStaffWriteRepository> _mockStaffWriteRepository;
        private readonly Mock<IScheduleReadRepository> _mockScheduleReadRepository;
        private readonly Mock<IScheduleWriteRepository> _mockScheduleWriteRepository;
        private readonly Mock<IPrisonStaffReadRepository> _mockPrisonStaffReadRepository;
        private readonly Mock<IPrisonStaffWriteRepository> _mockPrisonStaffWriteRepository;
        private readonly StaffService _staffService;

        public StaffServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockStaffReadRepository = new Mock<IStaffReadRepository>();
            _mockStaffWriteRepository = new Mock<IStaffWriteRepository>();
            _mockScheduleReadRepository = new Mock<IScheduleReadRepository>();
            _mockScheduleWriteRepository = new Mock<IScheduleWriteRepository>();
            _mockPrisonStaffReadRepository = new Mock<IPrisonStaffReadRepository>();
            _mockPrisonStaffWriteRepository = new Mock<IPrisonStaffWriteRepository>();

            _mockUnitOfWork.Setup(u => u.GetRepository<IStaffReadRepository>()).Returns(_mockStaffReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IStaffWriteRepository>()).Returns(_mockStaffWriteRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IScheduleReadRepository>()).Returns(_mockScheduleReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IScheduleWriteRepository>()).Returns(_mockScheduleWriteRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonStaffReadRepository>()).Returns(_mockPrisonStaffReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonStaffWriteRepository>()).Returns(_mockPrisonStaffWriteRepository.Object);

            _staffService = new StaffService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region CreateStaffAsync Tests

        [Fact]
        public async Task CreateStaffAsync_CreatesStaffSuccessfully()
        {
            // Arrange
            var createStaffDto = new CreateStaffDto
            {
                Name = "New Staff",
                Email = "new@example.com",
                PrisonId = Guid.NewGuid(),
                Position = PositionType.Nurse,
                DateOfStarting = DateTime.Today,
                Schedules = new List<ScheduleDto>
                {
                    new ScheduleDto { Date = DateTime.Today, ShiftType = ShiftType.Morning }
                }
            };

            _mockStaffReadRepository.Setup(r => r.AnyAsync(
                It.Is<Expression<Func<Staff, bool>>>(expr => expr.Compile().Invoke(new Staff { Email = createStaffDto.Email })),
                It.IsAny<bool>()))
                .ReturnsAsync(false);

            _mockMapper.Setup(m => m.Map<Staff>(It.IsAny<CreateStaffDto>()))
                .Returns((CreateStaffDto dto) => new Staff
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Email = dto.Email,
                    Position = dto.Position
                });

            // Act
            var result = await _staffService.CreateStaffAsync(createStaffDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("Staff member created successfully", result.Messages.First());

            _mockStaffWriteRepository.Verify(r => r.AddAsync(It.Is<Staff>(s =>
                s.Name == createStaffDto.Name &&
                s.Email == createStaffDto.Email &&
                s.Position == createStaffDto.Position)), Times.Once);

            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateStaffAsync_ReturnsError_WhenEmailExists()
        {
            // Arrange
            var createStaffDto = new CreateStaffDto
            {
                Email = "existing@example.com",
                Position = PositionType.Nurse
            };

            _mockStaffReadRepository.Setup(r => r.AnyAsync(
                It.Is<Expression<Func<Staff, bool>>>(x =>
                    x.Compile().Invoke(new Staff { Email = createStaffDto.Email })),
                It.IsAny<bool>()))
                .ReturnsAsync(true);

            // Act
            var result = await _staffService.CreateStaffAsync(createStaffDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("An employee with this email already exists", result.Messages.First());
        }

        [Fact]
        public async Task CreateStaffAsync_ReturnsError_ForInvalidPosition()
        {
            // Arrange
            var createStaffDto = new CreateStaffDto
            {
                Position = PositionType.Warden
            };

            // Act
            var result = await _staffService.CreateStaffAsync(createStaffDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Staff can only be created for specific positions", result.Messages.First());
        }

        [Fact]
        public async Task CreateStaffAsync_ReturnsError_WhenScheduleDateMismatch()
        {
            // Arrange
            var createStaffDto = new CreateStaffDto
            {
                Position = PositionType.Nurse,
                DateOfStarting = DateTime.Today,
                Schedules = new List<ScheduleDto>
                {
                    new ScheduleDto { Date = DateTime.Today.AddDays(1) }
                }
            };

            // Act
            var result = await _staffService.CreateStaffAsync(createStaffDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Schedule date must match the start date", result.Messages.First());
        }

        #endregion

        #region UpdateStaffAsync Tests

        [Fact]
        public async Task UpdateStaffAsync_WhenStaffExistsAndEmailUnique_ReturnsSuccess()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var updateDto = new UpdateStaffDto
            {
                Name = "Updated Name",
                Email = "updated@email.com",
                PrisonId = Guid.NewGuid(),
                PhoneNumber = "1234567890",
                DateOfStarting = DateTime.Now,
                UserId = "user123"
            };

            var existingStaff = new Staff { Id = staffId, Email = "old@email.com" };
            var existingPrisonStaff = new PrisonStaff { StaffId = staffId, PrisonId = Guid.NewGuid() };

            _mockStaffReadRepository.Setup(x => x.GetByIdAsync(staffId, false, false))
                .ReturnsAsync(existingStaff);

            _mockStaffReadRepository.Setup(x => x.AnyAsync(
                    It.IsAny<Expression<Func<Staff, bool>>>(),
                    false
                ))
                .ReturnsAsync(false);

            _mockPrisonStaffReadRepository.Setup(x => x.GetSingleAsync(
                    It.IsAny<Expression<Func<PrisonStaff, bool>>>(),
                    null,
                    false,
                    false
                ))
                .ReturnsAsync(existingPrisonStaff);

            _mockUnitOfWork.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _staffService.UpdateStaffAsync(staffId, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _mockStaffWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<Staff>()), Times.Once);
            _mockPrisonStaffWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<PrisonStaff>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStaffAsync_WhenStaffNotFound_ReturnsFailure()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var updateDto = new UpdateStaffDto();

            _mockStaffReadRepository.Setup(x => x.GetByIdAsync(staffId, false, false))
                .ReturnsAsync((Staff)null);

            // Act
            var result = await _staffService.UpdateStaffAsync(staffId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Staff member not found", result.Messages);
            _mockStaffWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<Staff>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateStaffAsync_WhenEmailExists_ReturnsFailure()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var updateDto = new UpdateStaffDto
            {
                Email = "existing@email.com"
            };

            var existingStaff = new Staff { Id = staffId, Email = "old@email.com" };

            _mockStaffReadRepository.Setup(x => x.GetByIdAsync(staffId, false, false))
                .ReturnsAsync(existingStaff);
            _mockStaffReadRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Staff, bool>>>(), false))
                .ReturnsAsync(true);

            // Act
            var result = await _staffService.UpdateStaffAsync(staffId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("This email already belongs to another staff member", result.Messages);
            _mockStaffWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<Staff>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateStaffAsync_WhenPrisonStaffNotFound_UpdatesOnlyStaff()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var updateDto = new UpdateStaffDto
            {
                Name = "Updated Name",
                Email = "updated@email.com",
                PrisonId = Guid.NewGuid()
            };

            var existingStaff = new Staff { Id = staffId, Email = "old@email.com" };

            _mockStaffReadRepository.Setup(x => x.GetByIdAsync(staffId, false, false))
                .ReturnsAsync(existingStaff);
            _mockStaffReadRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Staff, bool>>>(), false))
                .ReturnsAsync(false);
            _mockPrisonStaffReadRepository.Setup(x => x.GetSingleAsync(
                It.IsAny<Expression<Func<PrisonStaff, bool>>>(),
                null,
                false,
                false
            ));
            _mockUnitOfWork.Setup(x => x.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _staffService.UpdateStaffAsync(staffId, updateDto);

            // Assert
            Assert.True(result.Success);
            _mockStaffWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<Staff>()), Times.Once);
            _mockPrisonStaffWriteRepository.Verify(x => x.UpdateAsync(It.IsAny<PrisonStaff>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        #endregion

        #region GetStaff Tests

        [Fact]
        public async Task GetStaffByIdAsync_ReturnsNotFoundWhenStaffDoesNotExist()
        {
            // Arrange
            var staffId = Guid.NewGuid();

            _mockStaffReadRepository.Setup(r => r.GetSingleAsync(
                It.IsAny<Expression<Func<Staff, bool>>>(),
                It.IsAny<Func<IQueryable<Staff>, IIncludableQueryable<Staff, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync((Staff)null);

            // Act
            var result = await _staffService.GetStaffByIdAsync(staffId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Staff member not found", result.Messages.First());
        }

        [Fact]
        public async Task GetAllStaffAsync_ReturnsEmptyListWhenNoStaffFound()
        {
            // Arrange
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var emptyStaffList = new List<Staff>();

            _mockStaffReadRepository.Setup(r => r.GetAllByPagingAsync(
                It.IsAny<Expression<Func<Staff, bool>>>(),
                It.IsAny<Func<IQueryable<Staff>, IIncludableQueryable<Staff, object>>>(),
                It.IsAny<Func<IQueryable<Staff>, IOrderedQueryable<Staff>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
                .ReturnsAsync(emptyStaffList);

            _mockStaffReadRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<Staff, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync(0);

            // Act
            var result = await _staffService.GetAllStaffAsync(paginationRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Empty(result.Data.Data);
            Assert.Equal("No staff members found", result.Messages.First());
        }

        #endregion

        #region DeleteStaffAsync Tests

        [Fact]
        public async Task DeleteStaffAsync_ReturnsNotFoundWhenStaffDoesNotExist()
        {
            // Arrange
            var staffId = Guid.NewGuid();

            _mockStaffReadRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, false))
                .ReturnsAsync((Staff)null);

            // Act
            var result = await _staffService.DeleteStaffAsync(staffId, false);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Staff member not found", result.Messages.First());
        }

        #endregion
    }
}