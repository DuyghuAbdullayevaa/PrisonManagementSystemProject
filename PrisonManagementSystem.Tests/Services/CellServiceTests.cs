using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Implementations;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using Xunit;

namespace PrisonManagementSystem.Tests.Services
{
    public class CellServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICellReadRepository> _mockCellReadRepository;
        private readonly Mock<ICellWriteRepository> _mockCellWriteRepository;
        private readonly Mock<IPrisonerReadRepository> _mockPrisonerReadRepository;
        private readonly Mock<IPrisonReadRepository> _mockPrisonReadRepository;
        private readonly CellService _cellService;

        public CellServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockCellReadRepository = new Mock<ICellReadRepository>();
            _mockCellWriteRepository = new Mock<ICellWriteRepository>();
            _mockPrisonerReadRepository = new Mock<IPrisonerReadRepository>();
            _mockPrisonReadRepository = new Mock<IPrisonReadRepository>();

            SetupUnitOfWorkRepositories();
            _cellService = new CellService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        private void SetupUnitOfWorkRepositories()
        {
            _mockUnitOfWork.Setup(u => u.GetRepository<ICellReadRepository>()).Returns(_mockCellReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<ICellWriteRepository>()).Returns(_mockCellWriteRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonerReadRepository>()).Returns(_mockPrisonerReadRepository.Object);
            _mockUnitOfWork.Setup(u => u.GetRepository<IPrisonReadRepository>()).Returns(_mockPrisonReadRepository.Object);
        }

        #region Helper Methods
        private Cell CreateTestCell(Guid? id = null, Guid? prisonId = null, string cellNumber = "A1",
                                  int capacity = 5, List<Prisoner> prisoners = null)
        {
            return new Cell
            {
                Id = id ?? Guid.NewGuid(),
                PrisonId = prisonId ?? Guid.NewGuid(),
                CellNumber = cellNumber,
                Capacity = capacity,
                Prisoners = prisoners ?? new List<Prisoner>()
            };
        }

        private Prison CreateTestPrison(Guid? id = null, int capacity = 100)
        {
            return new Prison
            {
                Id = id ?? Guid.NewGuid(),
                Capacity = capacity
            };
        }

        private CreateCellDto CreateTestCreateCellDto(Guid? prisonId = null, string cellNumber = "A1", int capacity = 5)
        {
            return new CreateCellDto
            {
                PrisonId = prisonId ?? Guid.NewGuid(),
                CellNumber = cellNumber,
                Capacity = capacity
            };
        }

        private UpdateCellDto CreateTestUpdateCellDto(Guid? prisonId = null, string cellNumber = "A1",
                                                    int capacity = 5, CellStatus status = CellStatus.Available)
        {
            return new UpdateCellDto
            {
                PrisonId = prisonId ?? Guid.NewGuid(),
                CellNumber = cellNumber,
                Capacity = capacity,
                Status = status
            };
        }
        #endregion
        #region GetAllCellsAsync Tests
        [Fact]
        public async Task GetAllCellsAsync_WithPagination_ReturnsCorrectData()
        {
            // Arrange
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };

            var cells = new List<Cell>
            {
                  CreateTestCell(cellNumber: "A1", capacity: 5),
                  CreateTestCell(cellNumber: "A2", capacity: 3)
            };

            _mockCellReadRepository.Setup(r => r.GetAllByPagingAsync(
                    It.IsAny<Expression<Func<Cell, bool>>>(),
                    It.IsAny<Func<IQueryable<Cell>, IIncludableQueryable<Cell, object>>>(),
                    It.IsAny<Func<IQueryable<Cell>, IOrderedQueryable<Cell>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(cells);

            _mockCellReadRepository.Setup(r => r.GetCountAsync(
                    It.IsAny<Expression<Func<Cell, bool>>>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(2);

            // Act
            var result = await _cellService.GetAllCellsAsync(paginationRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Data.Count());

            _mockCellReadRepository.Verify(r => r.GetAllByPagingAsync(
                It.IsAny<Expression<Func<Cell, bool>>>(),
                It.IsAny<Func<IQueryable<Cell>, IIncludableQueryable<Cell, object>>>(),
                It.IsAny<Func<IQueryable<Cell>, IOrderedQueryable<Cell>>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()),
                Times.Once);

            _mockCellReadRepository.Verify(r => r.GetCountAsync(
                It.IsAny<Expression<Func<Cell, bool>>>(),
                It.IsAny<bool>()),
                Times.Once);
        }

        #endregion

        #region GetCellByIdAsync Tests
        [Fact]
        public async Task GetCellByIdAsync_WhenCellExists_ReturnsCellWithDetails()
        {
            // Arrange
            var cellId = Guid.NewGuid();
            var expectedCell = CreateTestCell(cellId);

            _mockCellReadRepository.Setup(r => r.GetSingleAsync(
                It.IsAny<Expression<Func<Cell, bool>>>(),
                It.IsAny<Func<IQueryable<Cell>, IIncludableQueryable<Cell, object>>>(),
                false, false))
                .ReturnsAsync(expectedCell);

            // Act
            var result = await _cellService.GetCellByIdAsync(cellId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(cellId, result.Data.Id);

        }

        [Fact]
        public async Task GetCellByIdAsync_WhenCellDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var cellId = Guid.NewGuid();

            _mockCellReadRepository.Setup(r => r.GetSingleAsync(
                It.IsAny<Expression<Func<Cell, bool>>>(),
                It.IsAny<Func<IQueryable<Cell>, IIncludableQueryable<Cell, object>>>(),
                false, false))
                .ReturnsAsync((Cell)null);

            // Act
            var result = await _cellService.GetCellByIdAsync(cellId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Cell not found", result.Messages.First());
        }
        #endregion

        [Fact]
        public async Task AddCellAsync_WhenPrisonNotFound_ReturnsBadRequest()
        {
            // Arrange
            var createDto = CreateTestCreateCellDto();

            _mockPrisonReadRepository.Setup(r => r.GetByIdAsync(createDto.PrisonId, false, false))
                .ReturnsAsync((Prison)null);

            // Act
            var result = await _cellService.AddCellAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("not found", result.Messages.First());
        }

        [Fact]
        public async Task AddCellAsync_WhenCapacityExceedsPrisonLimit_ReturnsBadRequest()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var createDto = CreateTestCreateCellDto(prisonId, capacity: 20);
            var prison = CreateTestPrison(prisonId, 10);

            _mockPrisonReadRepository.Setup(r => r.GetByIdAsync(prisonId, false, false))
                .ReturnsAsync(prison);

            _mockCellReadRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Cell, bool>>>(), null, null, false, false))
                .ReturnsAsync(new List<Cell>());

            _mockCellReadRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Cell, bool>>>(), false))
                .ReturnsAsync(false);

            // Act
            var result = await _cellService.AddCellAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("capacity", result.Messages.First().ToLower());
        }


        #region UpdateCellAsync Tests
        [Fact]
        public async Task UpdateCellAsync_WhenDataValid_UpdatesCellSuccessfully()
        {
            // Arrange
            var prisonId = Guid.NewGuid();
            var cellId = Guid.NewGuid();
            var updateDto = CreateTestUpdateCellDto(prisonId);
            var existingCell = CreateTestCell(cellId, prisonId);
            var prison = CreateTestPrison(prisonId, 20);
            var existingCells = new List<Cell> { existingCell };

            _mockCellReadRepository.Setup(r => r.GetByIdAsync(cellId, false, false))
                .ReturnsAsync(existingCell);

            _mockPrisonReadRepository.Setup(r => r.GetByIdAsync(prisonId, false, false))
                .ReturnsAsync(prison);

            _mockCellReadRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Cell, bool>>>(), null, null, false, false))
                .ReturnsAsync(existingCells);

            _mockCellWriteRepository.Setup(r => r.UpdateAsync(existingCell)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _cellService.UpdateCellAsync(cellId, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _mockCellWriteRepository.Verify(r => r.UpdateAsync(existingCell), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCellAsync_WhenCellNotFound_ReturnsNotFound()
        {
            // Arrange
            var cellId = Guid.NewGuid();
            var updateDto = CreateTestUpdateCellDto();

            _mockCellReadRepository.Setup(r => r.GetByIdAsync(cellId, false, false))
                .ReturnsAsync((Cell)null);

            // Act
            var result = await _cellService.UpdateCellAsync(cellId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Cell not found", result.Messages.First());
        }
        #endregion

        #region DeleteCellAsync Tests
        [Fact]
        public async Task DeleteCellAsync_WhenSoftDelete_Succeeds()
        {
            // Arrange
            var cellId = Guid.NewGuid();
            var cell = CreateTestCell(cellId, prisoners: new List<Prisoner>
            {
                new Prisoner { Status = PrisonerStatus.Released }
            });

            _mockCellReadRepository.Setup(r => r.GetSingleAsync(
                It.IsAny<Expression<Func<Cell, bool>>>(),
                It.IsAny<Func<IQueryable<Cell>, IIncludableQueryable<Cell, object>>>(),
                false, false))
                .ReturnsAsync(cell);

            _mockCellWriteRepository.Setup(r => r.DeleteAsync(cell, false)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _cellService.DeleteCellAsync(cellId, false);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Contains("soft deleted", result.Messages.First());
            _mockCellWriteRepository.Verify(r => r.DeleteAsync(cell, false), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCellAsync_WhenHardDeleteWithActivePrisoners_Fails()
        {
            // Arrange
            var cellId = Guid.NewGuid();
            var cell = CreateTestCell(cellId, prisoners: new List<Prisoner>
            {
                new Prisoner { Status = PrisonerStatus.Active }
            });

            _mockCellReadRepository.Setup(r => r.GetSingleAsync(
                It.IsAny<Expression<Func<Cell, bool>>>(),
                It.IsAny<Func<IQueryable<Cell>, IIncludableQueryable<Cell, object>>>(),
                false, false))
                .ReturnsAsync(cell);

            // Act
            var result = await _cellService.DeleteCellAsync(cellId, true);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("active prisoners", result.Messages.First());
        }
        #endregion


    }
}