using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a sale record in the system, including details about products, discounts, and status.
    /// This entity follows domain-driven design principles and includes business rules validation.
    /// </summary>
    public class Sale : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique number of the sale.
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the sale was made.
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        public string Customer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total amount of the sale.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the branch where the sale was made.
        /// </summary>
        public string Branch { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of items associated with the sale.
        /// </summary>
        public List<SaleItem> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets whether the sale is cancelled or active.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets the date and time when the sale was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets the date and time of the last update to the sale information.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the Sale class.
        /// </summary>
        public Sale()
        {
            CreatedAt = DateTime.UtcNow;
            IsCancelled = false;
        }

        /// <summary>
        /// Cancels the current sale, marking it as cancelled and setting the update timestamp.
        /// </summary>
        public void CancelSale()
        {
            IsCancelled = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds an item to the sale, applying business rules for discounts and quantity validation.
        /// </summary>
        /// <param name="item">The sale item to add.</param>
        public void AddItem(SaleItem item)
        {
            if (item.Quantity > 20)
                throw new InvalidOperationException("Cannot sell more than 20 identical items.");

            if (item.Quantity >= 10)
                item.Discount = 0.20m;
            else if (item.Quantity >= 4)
                item.Discount = 0.10m;
            else
                item.Discount = 0m;

            item.Total = item.Quantity * item.UnitPrice * (1 - item.Discount);
            Items.Add(item);
            UpdateTotalAmount();
        }

        /// <summary>
        /// Updates the total sale amount based on its items.
        /// </summary>
        public void UpdateTotalAmount()
        {
            TotalAmount = Items.Sum(i => i.Total);
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
