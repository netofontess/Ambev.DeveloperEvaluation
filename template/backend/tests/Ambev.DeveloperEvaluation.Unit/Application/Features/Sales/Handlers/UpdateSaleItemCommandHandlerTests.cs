using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using OneOf.Types;
using Xunit;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Unit.Application.Features.Sales.Handlers
{
    public class UpdateSaleItemCommandHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly UpdateSaleItemCommandHandler _handler;

        public UpdateSaleItemCommandHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _handler = new UpdateSaleItemCommandHandler(_saleRepository);
        }

        [Fact]
        public async Task Handle_ExistingSaleAndItem_ShouldUpdateItemAndReturnSaleDTO()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.AddItem(productId, "Product 1", 2, 100m, 0m);
            var item = sale.Items.First();

            var command = new UpdateSaleItemCommand
            {
                SaleId = saleId,
                ItemId = item.Id,
                Quantity = 3,
                UnitPrice = 150m,
                DiscountPercentage = 10m
            };

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult(sale));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().NotBeNull();
            result.AsT0.Id.Should().Be(sale.Id);
            result.AsT0.CustomerName.Should().Be("John Doe");
            result.AsT0.BranchName.Should().Be("Main Branch");
            result.AsT0.CustomerId.Should().Be(customerId);
            result.AsT0.BranchId.Should().Be(branchId);
            result.AsT0.UserId.Should().Be(userId);
            result.AsT0.IsCancelled.Should().BeFalse();
            result.AsT0.Items.Should().ContainSingle();

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_NonExistingSale_ShouldReturnNotFound()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var command = new UpdateSaleItemCommand
            {
                SaleId = saleId,
                ItemId = itemId,
                Quantity = 3,
                UnitPrice = 150m,
                DiscountPercentage = 10m
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
        public async Task Handle_NonExistingItem_ShouldReturnNotFound()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.AddItem(productId, "Product 1", 2, 100m, 0m);

            var command = new UpdateSaleItemCommand
            {
                SaleId = saleId,
                ItemId = itemId, // Using a different ID than the actual item
                Quantity = 3,
                UnitPrice = 150m,
                DiscountPercentage = 10m
            };

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult(sale));

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
            var productId = Guid.NewGuid();

            var sale = new Sale("John Doe", "Main Branch", customerId, branchId, userId);
            sale.AddItem(productId, "Product 1", 2, 100m, 0m);
            var item = sale.Items.First();
            sale.Cancel();

            var command = new UpdateSaleItemCommand
            {
                SaleId = saleId,
                ItemId = item.Id,
                Quantity = 3,
                UnitPrice = 150m,
                DiscountPercentage = 10m
            };

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult(sale));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT2.Should().BeTrue();
            result.AsT2.Message.Should().Be("Cannot update items in a cancelled sale");

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldReturnError()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var command = new UpdateSaleItemCommand
            {
                SaleId = saleId,
                ItemId = itemId,
                Quantity = 3,
                UnitPrice = 150m,
                DiscountPercentage = 10m
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