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
    public class CancelSaleCommandHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly CancelSaleCommandHandler _handler;

        public CancelSaleCommandHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _handler = new CancelSaleCommandHandler(_saleRepository);
        }

        [Fact]
        public async Task Handle_ExistingSale_ShouldCancelSaleAndReturnSaleDTO()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var command = new CancelSaleCommand { Id = saleId };

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
            result.AsT0.IsCancelled.Should().BeTrue();
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
            var command = new CancelSaleCommand { Id = saleId };

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
            var command = new CancelSaleCommand { Id = saleId };

            var sale = new Sale("John Doe", "Main Branch", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            sale.AddItem(Guid.NewGuid(), "Product 1", 2, 100m, 0m);
            sale.Cancel();

            _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
                .Returns(Task.FromResult(sale));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT2.Should().BeTrue();
            result.AsT2.Message.Should().Be("Sale is already cancelled");

            await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
            await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldReturnError()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var command = new CancelSaleCommand { Id = saleId };

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