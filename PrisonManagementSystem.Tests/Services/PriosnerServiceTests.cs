using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Implementations;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerCrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PrisonManagementSystem.Tests.Services
{
    public class PrisonerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPrisonerReadRepository> _mockPrisonerReadRepo;
        private readonly Mock<IPrisonerWriteRepository> _mockPrisonerWriteRepo;
        private readonly Mock<ICellReadRepository> _mockCellReadRepo;
        private readonly Mock<ICellWriteRepository> _mockCellWriteRepo;
        private readonly Mock<IPrisonReadRepository> _mockPrisonReadRepo;
        private readonly Mock<IPrisonWriteRepository> _mockPrisonWriteRepo;
        private readonly Mock<ICrimeWriteRepository> _mockCrimeWriteRepo;
        private readonly Mock<IPrisonerCrimeWriteRepository> _mockPrisonerCrimeWriteRepo;
        private readonly PrisonerService _prisonerService;

        public PrisonerServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            // Initialize repository mocks
            _mockPrisonerReadRepo = new Mock<IPrisonerReadRepository>();
            _mockPrisonerWriteRepo = new Mock<IPrisonerWriteRepository>();
            _mockCellReadRepo = new Mock<ICellReadRepository>();
            _mockCellWriteRepo = new Mock<ICellWriteRepository>();
            _mockPrisonReadRepo = new Mock<IPrisonReadRepository>();
            _mockPrisonWriteRepo = new Mock<IPrisonWriteRepository>();
            _mockCrimeWriteRepo = new Mock<ICrimeWriteRepository>();
            _mockPrisonerCrimeWriteRepo = new Mock<IPrisonerCrimeWriteRepository>();

            // Setup UnitOfWork repositories
            SetupUnitOfWorkRepositories();

            _prisonerService = new PrisonerService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        private void SetupUnitOfWorkRepositories()
        {
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonerReadRepository>()).Returns(_mockPrisonerReadRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonerWriteRepository>()).Returns(_mockPrisonerWriteRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<ICellReadRepository>()).Returns(_mockCellReadRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<ICellWriteRepository>()).Returns(_mockCellWriteRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonReadRepository>()).Returns(_mockPrisonReadRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonWriteRepository>()).Returns(_mockPrisonWriteRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<ICrimeWriteRepository>()).Returns(_mockCrimeWriteRepo.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonerCrimeWriteRepository>()).Returns(_mockPrisonerCrimeWriteRepo.Object);
        }

        [Fact]
        public async Task GetAllPrisonersAsync_ReturnsPrisoners_WhenPrisonersExist()
        {
            // Arrange
            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var prisoners = new List<Prisoner>
            {
                new Prisoner { Id = Guid.NewGuid(), FirstName = "Namiq", LastName = "Semedli" },
                new Prisoner { Id = Guid.NewGuid(), FirstName = "Ferid", LastName = "Salahli" }
            };

            _mockPrisonerReadRepo.Setup(r => r.GetAllByPagingAsync(
                It.IsAny<Expression<Func<Prisoner, bool>>>(),
                It.Is<Func<IQueryable<Prisoner>, IIncludableQueryable<Prisoner, object>>>(include => include != null),
                It.Is<Func<IQueryable<Prisoner>, IOrderedQueryable<Prisoner>>>(orderBy => orderBy != null),
                false,
                false,
                request.PageNumber,
                request.PageSize))
            .ReturnsAsync(prisoners);

            _mockPrisonerReadRepo.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<Prisoner, bool>>>(),
                false))
            .ReturnsAsync(2);

            // Act
            var result = await _prisonerService.GetAllPrisonersAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal("Prisoners retrieved successfully", result.Messages.First());
        }

        [Fact]
        public async Task GetPrisonerByIdAsync_ReturnsPrisoner_WhenPrisonerExists()
        {
            // Arrange
            var prisonerId = Guid.NewGuid();
            var prisoner = new Prisoner { Id = prisonerId, FirstName = "John", LastName = "Doe" };

            _mockPrisonerReadRepo.Setup(r => r.GetSingleAsync(
                    It.IsAny<Expression<Func<Prisoner, bool>>>(),
                    It.IsAny<Func<IQueryable<Prisoner>, IIncludableQueryable<Prisoner, object>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(prisoner);

            // Act
            var result = await _prisonerService.GetPrisonerByIdAsync(prisonerId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(prisonerId, result.Data.Id);
        }

        [Fact]
        public async Task GetPrisonerByIdAsync_ReturnsNotFound_WhenPrisonerDoesNotExist()
        {
            // Arrange
            var prisonerId = Guid.NewGuid();
            _mockPrisonerReadRepo.Setup(r => r.GetSingleAsync(
                    It.IsAny<Expression<Func<Prisoner, bool>>>(),
                    It.IsAny<Func<IQueryable<Prisoner>, IIncludableQueryable<Prisoner, object>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((Prisoner)null);

            // Act
            var result = await _prisonerService.GetPrisonerByIdAsync(prisonerId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("Prisoner not found", result.Messages.First());
        }

        [Fact]
        public async Task AddPrisonerAsync_ReturnsBadRequest_WhenCellIsFull()
        {
            // Arrange
            var dto = new CreatePrisonerDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                CellId = Guid.NewGuid(),
                Crimes = new List<CrimeDto>()
            };

            var cell = new Cell
            {
                Id = dto.CellId,
                Status = CellStatus.Full,
                Capacity = 2,
                CurrentOccupancy = 2
            };

            _mockPrisonerReadRepo.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Prisoner, bool>>>(),
                null,
                null,
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(new List<Prisoner>());

            _mockCellReadRepo.Setup(r => r.GetByIdAsync(
                dto.CellId,
                false,
                false))
                .ReturnsAsync(cell);

            // Act
            var result = await _prisonerService.AddPrisonerAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Cell is full or under maintenance", result.Messages.First());

            _mockPrisonerWriteRepo.Verify(r => r.AddAsync(It.IsAny<Prisoner>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePrisonerAsync_ReturnsBadRequest_WhenChangingLifeSentenceStatus()
        {
            // Arrange
            var prisonerId = Guid.NewGuid();
            var dto = new UpdatePrisonerDto
            {
                Status = PrisonerStatus.Active
            };

            var existingPrisoner = new Prisoner
            {
                Id = prisonerId,
                Status = PrisonerStatus.LifeSentence
            };

            _mockPrisonerReadRepo.Setup(r => r.GetByIdAsync(prisonerId, false, false)).ReturnsAsync(existingPrisoner);

            // Act
            var result = await _prisonerService.UpdatePrisonerAsync(prisonerId, dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Cannot change status of life-sentenced prisoner", result.Messages.First());
        }

        [Fact]
        public async Task DeletePrisonerAsync_ReturnsSuccess_WhenSoftDelete()
        {
            // Arrange
            var prisonerId = Guid.NewGuid();
            var prisoner = new Prisoner
            {
                Id = prisonerId,
                Status = PrisonerStatus.Active,
                CellId = Guid.NewGuid()
            };

            var cell = new Cell { Id = prisoner.CellId, CurrentOccupancy = 1, Capacity = 2 };
            var prison = new Prison { Id = Guid.NewGuid(), CurrentInmates = 1 };

            _mockPrisonerReadRepo.Setup(r => r.GetByIdAsync(prisonerId, false, false)).ReturnsAsync(prisoner);
            _mockCellReadRepo.Setup(r => r.GetByIdAsync(prisoner.CellId, false, false)).ReturnsAsync(cell);
            _mockPrisonReadRepo.Setup(r => r.GetSingleAsync(
                  It.IsAny<Expression<Func<Prison, bool>>>(),
                  It.IsAny<Func<IQueryable<Prison>, IIncludableQueryable<Prison, object>>>(),
                  It.IsAny<bool>(),
                  It.IsAny<bool>()))
              .ReturnsAsync(prison);

            _mockPrisonerReadRepo.Setup(r => r.GetSingleAsync(
              It.IsAny<Expression<Func<Prisoner, bool>>>(),
              It.IsAny<Func<IQueryable<Prisoner>, IIncludableQueryable<Prisoner, object>>>(),
              It.IsAny<bool>(),
              It.IsAny<bool>()))
          .ReturnsAsync((Prisoner)null);

            _mockPrisonerWriteRepo.Setup(r => r.DeleteAsync(prisoner, false)).ReturnsAsync(true);

            // Act
            var result = await _prisonerService.DeletePrisonerAsync(prisonerId, false);

            // Assert
            Assert.True(result.Success);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Prisoner archived successfully", result.Messages.First());
        }

        [Fact]
        public async Task DeletePrisonerAsync_ReturnsBadRequest_WhenPrisonerIsDeceased()
        {
            // Arrange
            var prisonerId = Guid.NewGuid();
            var prisoner = new Prisoner
            {
                Id = prisonerId,
                Status = PrisonerStatus.Deceased
            };

            _mockPrisonerReadRepo.Setup(r => r.GetByIdAsync(prisonerId, false, false)).ReturnsAsync(prisoner);

            // Act
            var result = await _prisonerService.DeletePrisonerAsync(prisonerId, false);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Cannot delete released or deceased prisoner", result.Messages.First());
        }

        [Fact]
        public async Task UpdatePrisonerAsync_ReturnsNotFound_WhenPrisonerDoesNotExist()
        {
            // Arrange
            var prisonerId = Guid.NewGuid();
            var dto = new UpdatePrisonerDto { FirstName = "Updated" };

            _mockPrisonerReadRepo.Setup(r => r.GetByIdAsync(prisonerId, false, false))
                .ReturnsAsync((Prisoner)null);

            // Act
            var result = await _prisonerService.UpdatePrisonerAsync(prisonerId, dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("Prisoner not found", result.Messages.First());
        }

        [Fact]
        public async Task AddPrisonerAsync_RollsBackTransaction_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new CreatePrisonerDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Now,
                CellId = Guid.NewGuid(),
                Crimes = new List<CrimeDto>()
            };

            var cell = new Cell { Id = dto.CellId, Status = CellStatus.Available, Capacity = 2, CurrentOccupancy = 1 };

            _mockPrisonerReadRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Prisoner, bool>>>(), null, null, false, false))
                .ReturnsAsync(new List<Prisoner>());
            _mockCellReadRepo.Setup(r => r.GetByIdAsync(dto.CellId, false, false))
                .ReturnsAsync(cell);
            _mockPrisonerWriteRepo.Setup(r => r.AddAsync(It.IsAny<Prisoner>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _prisonerService.AddPrisonerAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
            _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Never);
        }
    }
}