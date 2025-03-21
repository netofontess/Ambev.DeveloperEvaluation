using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Features.Sales.Handlers
{
    public class UpdateSaleCommandHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly UpdateSaleCommandHandler _handler;

        public UpdateSaleCommandHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _handler = new UpdateSaleCommandHandler(_saleRepository);
        }

        [Fact]
        public async Task Handle_ExistingSale_ShouldUpdateSaleAndReturnSaleDTO()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateSaleCommand
            {
                Id = saleId,
                CustomerName = "Jane Doe",
                BranchName = "Branch 2",
                CustomerId = customerId,
                BranchId = branchId,
                UserId = userId
            };

            var sale = new Sale("John Doe", "Main Branch", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m, 0m);

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult(sale));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().NotBeNull();
            result.AsT0.Id.Should().Be(sale.Id);
            result.AsT0.CustomerName.Should().Be("Jane Doe");
            result.AsT0.BranchName.Should().Be("Branch 2");
            result.AsT0.CustomerId.Should().Be(customerId);
            result.AsT0.BranchId.Should().Be(branchId);
            result.AsT0.UserId.Should().Be(userId);
            result.AsT0.Items.Should().ContainSingle();

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.Received(1).UpdateAsync(sale, CancellationToken.None);
            await _saleRepository.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_NonExistingSale_ShouldReturnNotFound()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateSaleCommand
            {
                Id = saleId,
                CustomerName = "Jane Doe",
                BranchName = "New Branch",
                CustomerId = customerId,
                BranchId = branchId,
                UserId = userId
            };

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult<Sale>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<NotFound>();

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_CancelledSale_ShouldReturnError()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateSaleCommand
            {
                Id = saleId,
                CustomerName = "Jane Doe",
                BranchName = "New Branch",
                CustomerId = customerId,
                BranchId = branchId,
                UserId = userId
            };

            var sale = new Sale("John Doe", "Main Branch", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m, 0m);
            sale.Cancel();

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult(sale));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT2.Should().BeTrue();
            result.AsT2.Message.Should().Be("Cannot update a cancelled sale");

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldReturnError()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateSaleCommand
            {
                Id = saleId,
                CustomerName = "Jane Doe",
                BranchName = "New Branch",
                CustomerId = customerId,
                BranchId = branchId,
                UserId = userId
            };

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromException<Sale>(new Exception("Database error")));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT2.Should().BeTrue();
            result.AsT2.Message.Should().Be("Database error");

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }
    }
}