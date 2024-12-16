using MediatR;

namespace Ambev.DeveloperEvaluation.Application.GetSales
{
    /// <summary>
    /// Query to get a sale by its ID or sale number.
    /// </summary>
    public class GetSaleCommand : IRequest<GetSaleResult>
    {
        public Guid SaleId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
    }
}
