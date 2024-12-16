using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    /// <summary>
    /// Handles the request to cancel a sale.
    /// </summary>
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly ISaleRepository _saleRepository;

        public DeleteSaleHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId)
                ?? throw new KeyNotFoundException("Sale not found.");

            sale.IsCancelled = true;
            sale.UpdatedAt = DateTime.UtcNow;

            await _saleRepository.UpdateAsync(sale);

            return Unit.Value;
        }
    }
}
