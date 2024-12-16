namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    /// <summary>
    /// Request to retrieve a sale by its ID.
    /// </summary>
    public class GetSaleRequest
    {
        public Guid Id { get; set; }
    }
}
