using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    /// <summary>
    /// Handles the request to update a sale.
    /// </summary>
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Unit>
    {
        private readonly ISaleRepository _saleRepository;

        public UpdateSaleHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<Unit> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId)
                ?? throw new KeyNotFoundException("Sale not found.");

            sale.Customer = request.Customer;
            sale.Branch = request.Branch;

            sale.Items.Clear();
            foreach (var item in request.Items)
            {
                sale.Items.Add(new SaleItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.Quantity * item.UnitPrice
                });
            }

            sale.TotalAmount = sale.Items.Sum(i => i.Total);
            sale.UpdatedAt = DateTime.UtcNow;

            await _saleRepository.UpdateAsync(sale);

            return Unit.Value;  
        }
    }
}
