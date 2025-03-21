using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Queries;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using OneOf.Types;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Features.Sales.Handlers;

public class GetSaleByIdQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly GetSaleByIdQueryHandler _handler;
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    public GetSaleByIdQueryHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new GetSaleByIdQueryHandler(_saleRepository);
    }

    [Fact]
    public async Task Handle_ExistingSale_ShouldReturnSaleDTO()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleByIdQuery(saleId);

        var sale = new Sale("John Doe", "Main Branch", _customerId, _branchId, _userId);
        sale.AddItem(_productId, "Product 1", 2, 100m, 0m);

        _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
            .Returns(Task.FromResult(sale));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsT0.Should().BeTrue();
        result.AsT0.Should().NotBeNull();
        result.AsT0.Id.Should().Be(sale.Id);
        result.AsT0.CustomerName.Should().Be("John Doe");
        result.AsT0.BranchName.Should().Be("Main Branch");
        result.AsT0.CustomerId.Should().Be(_customerId);
        result.AsT0.BranchId.Should().Be(_branchId);
        result.AsT0.UserId.Should().Be(_userId);
        result.AsT0.IsCancelled.Should().BeFalse();
        result.AsT0.TotalAmount.Should().Be(200m);
        result.AsT0.Items.Should().ContainSingle();

        await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_NonExistingSale_ShouldReturnNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleByIdQuery(saleId);

        _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
            .Returns(Task.FromResult<Sale>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<NotFound>();

        await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldReturnError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var query = new GetSaleByIdQuery(saleId);

        _saleRepository.GetByIdAsync(saleId, CancellationToken.None)
            .Returns(Task.FromException<Sale>(new Exception("Database error")));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsT2.Should().BeTrue();
        result.AsT2.Message.Should().Be("Database error");

        await _saleRepository.Received(1).GetByIdAsync(saleId, CancellationToken.None);
    }
}