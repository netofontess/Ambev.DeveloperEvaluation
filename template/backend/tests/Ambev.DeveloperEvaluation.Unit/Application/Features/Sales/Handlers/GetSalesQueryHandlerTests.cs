using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Queries;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Features.Sales.Handlers
{
    public class GetSalesQueryHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly GetSalesQueryHandler _handler;
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _branchId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        public GetSalesQueryHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _handler = new GetSalesQueryHandler(_saleRepository);
        }

        [Fact]
        public async Task Handle_ValidQuery_ShouldReturnSaleDTOs()
        {
            // Arrange
            var query = new GetSalesQuery
            {
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow,
                CustomerName = "John",
                CustomerId = _customerId,
                BranchId = _branchId,
                IsCancelled = false,
                MinAmount = 100,
                MaxAmount = 1000,
                OrderBy = "createdat desc",
                Page = 1,
                Size = 10
            };

            var sales = new List<Sale>
            {
                new Sale("John Doe", "Main Branch", _customerId, _branchId, _userId),
                new Sale("Jane Smith", "Branch 2", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
            };

            _saleRepository.GetSalesAsync(
                query.Page,
                query.Size,
                query.StartDate,
                query.EndDate,
                query.CustomerName,
                query.CustomerId,
                query.BranchId,
                query.IsCancelled,
                query.MinAmount,
                query.MaxAmount,
                query.OrderBy,
                CancellationToken.None)
                .Returns(sales);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().NotBeNull();
            result.AsT0.Should().HaveCount(2);

            await _saleRepository.Received(1).GetSalesAsync(
                query.Page,
                query.Size,
                query.StartDate,
                query.EndDate,
                query.CustomerName,
                query.CustomerId,
                query.BranchId,
                query.IsCancelled,
                query.MinAmount,
                query.MaxAmount,
                query.OrderBy,
                CancellationToken.None);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrows_ShouldReturnError()
        {
            // Arrange
            var query = new GetSalesQuery();

            _saleRepository.GetSalesAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<DateTime?>(),
                Arg.Any<DateTime?>(),
                Arg.Any<string>(),
                Arg.Any<Guid?>(),
                Arg.Any<Guid?>(),
                Arg.Any<bool?>(),
                Arg.Any<decimal?>(),
                Arg.Any<decimal?>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
                .Returns(Task.FromException<IEnumerable<Sale>>(new Exception("Database error")));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsT1.Should().BeTrue();
            result.AsT1.Message.Should().Be("Error retrieving sales: Database error");
        }
    }
}