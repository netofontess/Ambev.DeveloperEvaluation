namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Result of the CreateSaleCommand execution.
    /// </summary>
    public class CreateSaleResult
    {
        public Guid SaleId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsSuccessful { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
