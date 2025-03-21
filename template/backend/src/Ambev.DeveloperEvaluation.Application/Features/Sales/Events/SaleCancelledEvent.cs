using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Events
{
    public class SaleCancelledEvent : INotification
    {
        public string SaleId { get; }

        public SaleCancelledEvent(string saleId)
        {
            SaleId = saleId;
        }
    }
}