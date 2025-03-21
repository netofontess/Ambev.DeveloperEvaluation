using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Events
{
    public class SaleCreatedEvent : INotification
    {
        public string SaleId { get; }
        public string SaleNumber { get; }

        public SaleCreatedEvent(string saleId, string saleNumber)
        {
            SaleId = saleId;
            SaleNumber = saleNumber;
        }
    }
}