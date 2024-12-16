using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    /// <summary>
    /// Command to update an existing sale.
    /// </summary>
    public class UpdateSaleCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the sale to update.
        /// </summary>
        public Guid SaleId { get; set; }

        /// <summary>
        /// Gets or sets the updated customer name.
        /// </summary>
        public string Customer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the updated branch where the sale occurred.
        /// </summary>
        public string Branch { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the updated list of items in the sale.
        /// </summary>
        public List<UpdateSaleItemCommand> Items { get; set; } = new();
    }

    /// <summary>
    /// Represents an individual sale item within the UpdateSaleCommand.
    /// </summary>
    public class UpdateSaleItemCommand
    {
        /// <summary>
        /// Gets or sets the product name of the sale item.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity of the sale item.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price of the sale item.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
