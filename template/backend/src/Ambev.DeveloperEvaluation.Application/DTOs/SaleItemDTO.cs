using System;

namespace Ambev.DeveloperEvaluation.Application.DTOs
{
    public class SaleItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsCancelled { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}