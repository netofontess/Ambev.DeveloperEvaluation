using System;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Events
{
    public class SaleItemAddedEvent : INotification
    {
        public Guid SaleId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }

        public SaleItemAddedEvent(Guid saleId, Guid productId, int quantity, decimal unitPrice)
        {
            SaleId = saleId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}