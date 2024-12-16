using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    /// <summary>
    /// Command to delete (cancel) a sale.
    /// </summary>
    public class DeleteSaleCommand : IRequest<Unit>
    {
        public Guid SaleId { get; set; }
    }
}
