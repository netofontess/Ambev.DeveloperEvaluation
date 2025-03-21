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
using OneOf;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Features.Sales.Handlers
{
    public class CreateSaleCommandHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly CreateSaleCommandHandler _handler;

        public CreateSaleCommandHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _handler = new CreateSaleCommandHandler(_saleRepository);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateSaleAndReturnSaleDTO()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new CreateSaleCommand
            {
                CustomerName = "John Doe",
                BranchName = "Main Branch",
                CustomerId = customerId,
                BranchId = branchId,
                UserId = userId
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().NotBeNull();
            result.AsT0.CustomerName.Should().Be(command.CustomerName);
            result.AsT0.BranchName.Should().Be(command.BranchName);
            result.AsT0.CustomerId.Should().Be(customerId);
            result.AsT0.BranchId.Should().Be(branchId);
            result.AsT0.UserId.Should().Be(userId);
            result.AsT0.IsCancelled.Should().BeFalse();
            result.AsT0.Items.Should().BeEmpty();

            await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldReturnError()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var branchId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new CreateSaleCommand
            {
                CustomerName = "John Doe",
                BranchName = "Main Branch",
                CustomerId = customerId,
                BranchId = branchId,
                UserId = userId
            };

            _saleRepository.AddAsync(Arg.Any<Sale>(), CancellationToken.None)
                .Returns(Task.FromException<Sale>(new Exception("Database error")));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Message.Should().Be("Error creating sale: Database error");

            await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>(), CancellationToken.None);
            await _saleRepository.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        }
    }
}