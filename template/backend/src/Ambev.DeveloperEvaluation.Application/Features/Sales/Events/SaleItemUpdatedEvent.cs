using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Events
{
    public class SaleItemUpdatedEvent : INotification
    {
        public string SaleId { get; }
        public string ItemId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }

        public SaleItemUpdatedEvent(string saleId, string itemId, int quantity, decimal unitPrice)
        {
            SaleId = saleId;
            ItemId = itemId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}