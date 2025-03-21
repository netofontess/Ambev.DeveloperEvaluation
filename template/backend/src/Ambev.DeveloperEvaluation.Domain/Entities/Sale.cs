using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a sale in the system
    /// </summary>
    public class Sale : Entity
    {
        /// <summary>
        /// Gets or sets the customer's name
        /// </summary>
        public string CustomerName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the branch name
        /// </summary>
        public string BranchName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's unique identifier
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// Gets or sets the branch's unique identifier
        /// </summary>
        public Guid BranchId { get; private set; }

        /// <summary>
        /// Gets or sets the user's unique identifier who created the sale
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets or sets the status of the sale
        /// </summary>
        public string Status { get; private set; } = "Pending";

        /// <summary>
        /// Gets or sets whether the sale is cancelled
        /// </summary>
        public bool IsCancelled { get; private set; }

        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
        private readonly List<SaleItem> _items = new();

        protected Sale() { }

        public Sale(string customerName, string branchName, Guid customerId, Guid branchId, Guid userId)
        {
            CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
            BranchName = branchName ?? throw new ArgumentNullException(nameof(branchName));
            CustomerId = customerId;
            BranchId = branchId;
            UserId = userId;
            IsCancelled = false;
        }

        public void Update(string customerName, string branchName, Guid customerId, Guid branchId, Guid userId)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot update a cancelled sale");

            CustomerName = customerName;
            BranchName = branchName;
            CustomerId = customerId;
            BranchId = branchId;
            UserId = userId;
            SetUpdatedAt();
        }

        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("Sale is already cancelled");

            IsCancelled = true;
            Status = "Cancelled";
            SetUpdatedAt();
        }

        public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice, decimal discountPercentage)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot add items to a cancelled sale");

            var now = DateTime.UtcNow;
            SetUpdatedAt(now);
            var item = new SaleItem(this, productId, productName, quantity, unitPrice, discountPercentage);
            item.SetUpdatedAt(now);
            _items.Add(item);
        }

        public void UpdateItem(Guid itemId, int quantity, decimal unitPrice, decimal discountPercentage)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot update items in a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found");

            item.Update(quantity, unitPrice, discountPercentage);
            SetUpdatedAt();
        }

        public void CancelItem(Guid itemId)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot cancel items in a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new InvalidOperationException("Item not found");

            _items.Remove(item);
            SetUpdatedAt();
        }

        public decimal GetTotalAmount()
        {
            return _items.Sum(item => item.GetTotalAmount());
        }
    }
}