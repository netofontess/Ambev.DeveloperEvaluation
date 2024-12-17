using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    /// <summary>
    /// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
    /// </summary>
    public class CreateSaleHandlerTests
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ISaleService _saleService;
        private readonly CreateSaleHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
        /// Sets up the test dependencies and creates fake data generators.
        /// </summary>
        public CreateSaleHandlerTests()
        {
            _saleRepository = Substitute.For<ISaleRepository>();
            _saleService = Substitute.For<ISaleService>();
            _handler = new CreateSaleHandler(_saleRepository, _saleService);
        }

        /// <summary>
        /// Tests that a valid sale creation request is handled successfully.
        /// </summary>
        [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
        {
            // Given
            var command = new CreateSaleCommand
            {
                SaleNumber = "S12345",
                Customer = "John Doe",
                Branch = "Branch A",
                Items = new List<CreateSaleItemCommand>
        {
            new() { ProductName = "Product A", Quantity = 5, UnitPrice = 10.0m }
        }
            };

            Sale capturedSale = null; // Variável para capturar a entidade salva no repositório

            _saleRepository.AddAsync(Arg.Do<Sale>(s => capturedSale = s))
                .Returns(call =>
                {
                    capturedSale.Id = Guid.NewGuid(); // Simula o banco gerando o ID
                    return Task.CompletedTask;
                });

            // When
            var response = await _handler.Handle(command, CancellationToken.None);

            // Then
            response.Should().NotBeNull();
            response.SaleId.Should().Be(capturedSale.Id); // Compara com o ID gerado no mock
            response.SaleNumber.Should().Be(command.SaleNumber);

            _saleService.Received(1).ApplyBusinessRules(Arg.Any<Sale>());
            await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>());
        }

        /// <summary>
        /// Tests that the business rules are applied before saving the sale.
        /// </summary>
        [Fact(DisplayName = "Given sale creation request When handling Then applies business rules")]
        public async Task Handle_ValidRequest_AppliesBusinessRules()
        {
            // Given
            var command = new CreateSaleCommand
            {
                SaleNumber = "S12345",
                Customer = "John Doe",
                Branch = "Branch A",
                Items = new List<CreateSaleItemCommand>
            {
                new() { ProductName = "Product A", Quantity = 5, UnitPrice = 20.0m }
            }
            };

            // When
            await _handler.Handle(command, CancellationToken.None);

            // Then
            _saleService.Received(1).ApplyBusinessRules(Arg.Is<Sale>(s =>
                s.SaleNumber == command.SaleNumber &&
                s.Customer == command.Customer &&
                s.Items.Count == 1));
        }
    }
}
