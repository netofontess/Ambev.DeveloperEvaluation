namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale
{
    /// <summary>
    /// Request to delete (cancel) a sale.
    /// </summary>
    public class DeleteSaleRequest
    {
        public Guid Id { get; set; }
    }
}
