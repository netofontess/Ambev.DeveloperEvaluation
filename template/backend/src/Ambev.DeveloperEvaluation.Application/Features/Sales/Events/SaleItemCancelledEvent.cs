using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Events
{
    public class SaleItemCancelledEvent : INotification
    {
        public string SaleId { get; }
        public string ItemId { get; }

        public SaleItemCancelledEvent(string saleId, string itemId)
        {
            SaleId = saleId;
            ItemId = itemId;
        }
    }
}