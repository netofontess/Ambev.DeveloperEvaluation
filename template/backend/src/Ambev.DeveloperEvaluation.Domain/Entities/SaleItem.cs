using System;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : Entity
    {
        public Guid SaleId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsCancelled { get; private set; }
        public virtual Sale? Sale { get; private set; }

        protected SaleItem() { }

        public SaleItem(Sale sale, Guid productId, string productName, int quantity, decimal unitPrice, decimal discountPercentage)
        {
            Sale = sale ?? throw new ArgumentNullException(nameof(sale));
            SaleId = sale.Id;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            DiscountPercentage = discountPercentage;
            IsCancelled = false;
            CalculateTotalAmount();
            SetUpdatedAt(sale.UpdatedAt);
        }

        public void Update(int quantity, decimal unitPrice, decimal discountPercentage)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot update a cancelled item");

            Quantity = quantity;
            UnitPrice = unitPrice;
            DiscountPercentage = discountPercentage;
            CalculateTotalAmount();
            SetUpdatedAt();
        }

        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("Item is already cancelled");

            IsCancelled = true;
            SetUpdatedAt();
        }

        private void CalculateTotalAmount()
        {
            if (IsCancelled)
            {
                TotalAmount = 0;
                return;
            }

            var subtotal = Quantity * UnitPrice;
            var discount = subtotal * (DiscountPercentage / 100m);
            TotalAmount = subtotal - discount;
        }

        public decimal GetTotalAmount()
        {
            if (IsCancelled)
                return 0;

            var subtotal = Quantity * UnitPrice;
            var discount = subtotal * (DiscountPercentage / 100);
            return subtotal - discount;
        }
    }
}