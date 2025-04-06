using Moq;
using AutoMapper;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.BL.Services.Implementations;
using System.Linq;
using System;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
namespace PrisonManagementSystem.Tests.Services
{
    public class PrisonServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPrisonReadRepository> _mockPrisonReadRepository;
        private readonly Mock<IPrisonWriteRepository> _mockPrisonWriteRepository;
        private readonly Mock<IPrisonerReadRepository> _mockPrisonerReadRepository;
        private readonly Mock<IPrisonStaffReadRepository> _mockPrisonStaffReadRepository;
        private readonly Mock<ICellReadRepository> _mockCellReadRepository;
        private readonly Mock<ICellWriteRepository> _mockCellWriteRepository;
        private readonly PrisonService _prisonService;

        public PrisonServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockPrisonReadRepository = new Mock<IPrisonReadRepository>();
            _mockPrisonWriteRepository = new Mock<IPrisonWriteRepository>();
            _mockPrisonerReadRepository = new Mock<IPrisonerReadRepository>();
            _mockPrisonStaffReadRepository = new Mock<IPrisonStaffReadRepository>();
            _mockCellReadRepository = new Mock<ICellReadRepository>();
            _mockCellWriteRepository = new Mock<ICellWriteRepository>();

            SetupUnitOfWorkRepositories();
            _prisonService = new PrisonService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        private void SetupUnitOfWorkRepositories()
        {
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonReadRepository>()).Returns(_mockPrisonReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonWriteRepository>()).Returns(_mockPrisonWriteRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonerReadRepository>()).Returns(_mockPrisonerReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonStaffReadRepository>()).Returns(_mockPrisonStaffReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<ICellReadRepository>()).Returns(_mockCellReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<ICellWriteRepository>()).Returns(_mockCellWriteRepository.Object);
        }
        
        #region CreatePrisonAsync Tests

        [Fact]
        public async Task CreatePrisonAsync_ReturnsSuccess_WhenPrisonIsValid()
        {
            // Arrange
            var createDto = new CreatePrisonDto
            {
                Name = "New Prison",
                Location = "New Location",
                Capacity = 100,
                IsMalePrison = true,
                IsActive = true
            };

            var prison = new Prison
            {
                Name = createDto.Name,
                Location = createDto.Location,
                Capacity = createDto.Capacity,
                IsMalePrison = createDto.IsMalePrison,
                Status = createDto.IsActive ? PrisonStatus.Active : PrisonStatus.Closed
            };

            SetupPrisonReadRepository(null);
            _mockMapper.Setup(m => m.Map<Prison>(createDto)).Returns(prison);
            _mockPrisonWriteRepository.Setup(x => x.AddAsync(prison)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _prisonService.CreatePrisonAsync(createDto);

            // Assert
            AssertSuccessResult(result, 201, "Prison created successfully");
            _mockPrisonWriteRepository.Verify(x => x.AddAsync(It.IsAny<Prison>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Theory]
        [InlineData(null, "Location", 100, false, false)]
        [InlineData("Name", null, 100, false, false)]
        [InlineData("Name", "Location", 0, false, false)]
        [InlineData("Name", "Location", -1, false, false)]
        public async Task CreatePrisonAsync_ReturnsFailure_WhenInputInvalid(string name, string location, int capacity, bool isMale, bool isActive)
        {
            // Arrange
            var createDto = new CreatePrisonDto
            {
                Name = name,
                Location = location,
                Capacity = capacity,
                IsMalePrison = isMale,
                IsActive = isActive
            };

            // Act
            var result = await _prisonService.CreatePrisonAsync(createDto);

            // Assert
            AssertFailureResult(result, 400);
        }

        [Fact]
        public async Task CreatePrisonAsync_ReturnsFailure_WhenPrisonAlreadyExists()
        {
            // Arrange
            var createDto = new CreatePrisonDto { Name = "Existing Prison", Location = "Existing Location" };
            var existingPrison = new Prison { Name = createDto.Name, Location = createDto.Location };

            SetupPrisonReadRepository(existingPrison);

            // Act
            var result = await _prisonService.CreatePrisonAsync(createDto);

            // Assert
            AssertFailureResult(result, 400, "Prison with this name and location already exists");
            _mockPrisonWriteRepository.Verify(x => x.AddAsync(It.IsAny<Prison>()), Times.Never);
        }

        [Fact]
        public async Task CreatePrisonAsync_ReturnsFailure_WhenAddFails()
        {
            // Arrange
            var createDto = new CreatePrisonDto { Name = "New Prison", Location = "New Location" };
            var prison = new Prison { Name = createDto.Name, Location = createDto.Location };

            SetupPrisonReadRepository(null);
            _mockMapper.Setup(m => m.Map<Prison>(createDto)).Returns(prison);
            _mockPrisonWriteRepository.Setup(x => x.AddAsync(prison)).ReturnsAsync(false);

            // Act
            var result = await _prisonService.CreatePrisonAsync(createDto);

            // Assert
            AssertFailureResult(result, 400, "Failed to create prison");
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Never);
        }

        #endregion

        #region UpdatePrisonAsync Tests

        [Fact]
        public async Task UpdatePrisonAsync_ReturnsSuccess_WhenPrisonIsValid()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var updateDto = new UpdatePrisonDto
            {
                Name = "Updated Prison",
                Location = "Updated Location",
                Capacity = 200,
                IsMalePrison = false,
                IsActive = true
            };

            var existingPrison = new Prison
            {
                Id = prisonId,
                Name = "Original Prison",
                Location = "Original Location",
                Capacity = 100,
                IsMalePrison = true,
                Status = PrisonStatus.Active
            };

            SetupGetByIdAsync(prisonId, existingPrison);
            _mockMapper.Setup(m => m.Map(updateDto, existingPrison)).Callback<UpdatePrisonDto, Prison>((dto, p) =>
            {
                p.Name = dto.Name;
                p.Location = dto.Location;
                p.Capacity = dto.Capacity;
                p.IsMalePrison = dto.IsMalePrison;
                p.Status = dto.IsActive ? PrisonStatus.Active : PrisonStatus.Closed;
            });
            _mockPrisonWriteRepository.Setup(r => r.UpdateAsync(existingPrison)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _prisonService.UpdatePrisonAsync(prisonId, updateDto);

            // Assert
            AssertSuccessResult(result, 200);
            Assert.Equal("Updated Prison", existingPrison.Name);
            Assert.Equal("Updated Location", existingPrison.Location);
            Assert.Equal(200, existingPrison.Capacity);
            Assert.False(existingPrison.IsMalePrison);
            Assert.Equal(PrisonStatus.Active, existingPrison.Status);
        }

        [Fact]
        public async Task UpdatePrisonAsync_ReturnsNotFound_WhenPrisonDoesNotExist()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            SetupGetByIdAsync(prisonId, null);

            // Act
            var result = await _prisonService.UpdatePrisonAsync(prisonId, new UpdatePrisonDto());

            // Assert
            AssertFailureResult(result, 404, "Prison not found");
        }

        [Fact]
        public async Task UpdatePrisonAsync_ReturnsFailure_WhenClosingPrisonWithActivePrisoners()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var updateDto = new UpdatePrisonDto { IsActive = false };
            var existingPrison = new Prison { Id = prisonId, Status = PrisonStatus.Active };

            SetupGetByIdAsync(prisonId, existingPrison);
            _mockPrisonerReadRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<Prisoner, bool>>>(), false))
                .ReturnsAsync(1);

            // Act
            var result = await _prisonService.UpdatePrisonAsync(prisonId, updateDto);

            // Assert
            AssertFailureResult(result, 400, "Cannot close prison with active prisoners or staff");
        }

        [Fact]
        public async Task UpdatePrisonAsync_ReturnsFailure_WhenUpdatingClosedPrison()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var updateDto = new UpdatePrisonDto { IsActive = true };
            var closedPrison = new Prison { Id = prisonId, Status = PrisonStatus.Closed };

            SetupGetByIdAsync(prisonId, closedPrison);

            // Act
            var result = await _prisonService.UpdatePrisonAsync(prisonId, updateDto);

            // Assert
            AssertFailureResult(result, 400, "Cannot update a closed prison");
        }

        [Fact]
        public async Task UpdatePrisonAsync_ReturnsFailure_WhenUpdateFails()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var updateDto = new UpdatePrisonDto();
            var existingPrison = new Prison { Id = prisonId, Status = PrisonStatus.Active };

            SetupGetByIdAsync(prisonId, existingPrison);
            _mockMapper.Setup(m => m.Map(updateDto, existingPrison));
            _mockPrisonWriteRepository.Setup(r => r.UpdateAsync(existingPrison)).ReturnsAsync(false);

            // Act
            var result = await _prisonService.UpdatePrisonAsync(prisonId, updateDto);

            // Assert
            AssertFailureResult(result, 400, "Failed to update prison");
        }

        #endregion

        #region DeletePrisonAsync Tests

        [Fact]
        public async Task DeletePrisonAsync_ReturnsSuccess_WhenSoftDeleteWithNoDependencies()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var prison = new Prison { Id = prisonId, Status = PrisonStatus.Active };
            var isHardDelete = false;

            SetupDeleteTest(prisonId, prison, isHardDelete, hasStaff: false, hasCells: false);
            _mockPrisonWriteRepository.Setup(r => r.DeleteAsync(prison, isHardDelete)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _prisonService.DeletePrisonAsync(prisonId, isHardDelete);

            // Assert
            AssertSuccessResult(result, 200);
        }

        [Fact]
        public async Task DeletePrisonAsync_ReturnsSuccess_WhenHardDeleteWithNoDependencies()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var prison = new Prison { Id = prisonId };
            var isHardDelete = true;

            SetupDeleteTest(prisonId, prison, isHardDelete, hasStaff: false, hasCells: false);
            _mockPrisonWriteRepository.Setup(r => r.DeleteAsync(prison, isHardDelete)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _prisonService.DeletePrisonAsync(prisonId, isHardDelete);

            // Assert
            AssertSuccessResult(result, 200, "Prison permanently deleted successfully");
        }

        [Fact]
        public async Task DeletePrisonAsync_ReturnsFailure_WhenPrisonHasActiveStaff()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var prison = new Prison { Id = prisonId };
            var isHardDelete = false;

            SetupDeleteTest(prisonId, prison, isHardDelete, hasStaff: true);

            // Act
            var result = await _prisonService.DeletePrisonAsync(prisonId, isHardDelete);

            // Assert
            AssertFailureResult(result, 400, "Cannot delete prison with active staff");
        }



        #endregion

        #region ChangePrisonStatusAsync Tests

        [Fact]
        public async Task ChangePrisonStatusAsync_ReturnsSuccess_WhenActivatingPrison()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var isActive = true;
            var prison = new Prison { Id = prisonId, Status = PrisonStatus.Closed };

            SetupGetByIdAsync(prisonId, prison);
            _mockPrisonWriteRepository.Setup(r => r.UpdateAsync(prison)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _prisonService.ChangePrisonStatusAsync(prisonId, isActive);

            // Assert
            AssertSuccessResult(result, 200);
            Assert.Equal(PrisonStatus.Active, prison.Status);
        }

        [Fact]
        public async Task ChangePrisonStatusAsync_ReturnsFailure_WhenClosingPrisonWithActivePrisoners()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var isActive = false;
            var prison = new Prison { Id = prisonId, Status = PrisonStatus.Active };

            SetupGetByIdAsync(prisonId, prison);
            _mockPrisonerReadRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Prisoner, bool>>>(), false))
                .ReturnsAsync(true);

            // Act
            var result = await _prisonService.ChangePrisonStatusAsync(prisonId, isActive);

            // Assert
            AssertFailureResult(result, 400, "Cannot change status of prison with active prisoners");
        }

        [Fact]
        public async Task ChangePrisonStatusAsync_ReturnsFailure_WhenClosingPrisonWithActiveStaff()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var isActive = false;
            var prison = new Prison { Id = prisonId, Status = PrisonStatus.Active };

            SetupGetByIdAsync(prisonId, prison);
            _mockPrisonerReadRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Prisoner, bool>>>(), false))
                .ReturnsAsync(false);
            _mockPrisonStaffReadRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<PrisonStaff, bool>>>(), false))
                .ReturnsAsync(true);

            // Act
            var result = await _prisonService.ChangePrisonStatusAsync(prisonId, isActive);

            // Assert
            AssertFailureResult(result, 400, "Cannot close prison with active staff");
        }

        #endregion

        #region Helper Methods

        private void SetupPrisonReadRepository(Prison returnValue)
        {
            _mockPrisonReadRepository.Setup(x => x.GetSingleAsync(
                It.IsAny<Expression<Func<Prison, bool>>>(),
                null,
                false,
                false
            )).ReturnsAsync(returnValue);
        }

        private void SetupGetByIdAsync(Guid id, Prison returnValue, bool enableTracking = true, bool isDeleted = false)
        {
            _mockPrisonReadRepository.Setup(r => r.GetByIdAsync(
                id,
                enableTracking,
                isDeleted
            )).ReturnsAsync(returnValue);
        }

        private void SetupDeleteTest(Guid prisonId, Prison prison, bool isHardDelete, bool hasStaff = false, bool hasCells = false)
        {
            SetupGetByIdAsync(prisonId, prison);
            _mockPrisonStaffReadRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<PrisonStaff, bool>>>(), false))
                .ReturnsAsync(hasStaff);

            var cells = hasCells ? new List<Cell> { new Cell { Id = Guid.NewGuid(), PrisonId = prisonId } } : new List<Cell>();
            _mockCellReadRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Cell, bool>>>(), null, null, false, false))
                .ReturnsAsync(cells);
        }

        private void AssertSuccessResult(GenericResponseModel<bool> result, int expectedStatusCode, string expectedMessage = null)
        {
            Assert.True(result.Success);
            Assert.Equal(expectedStatusCode, result.StatusCode);
            Assert.True(result.Data);
            if (expectedMessage != null)
            {
                Assert.Contains(expectedMessage, result.Messages);
            }
        }

        private void AssertFailureResult(GenericResponseModel<bool> result, int expectedStatusCode, string expectedMessage = null)
        {
            Assert.False(result.Success);
            Assert.Equal(expectedStatusCode, result.StatusCode);
            if (expectedMessage != null)
            {
                Assert.Contains(expectedMessage, result.Messages);
            }
        }

        #endregion
    }
}