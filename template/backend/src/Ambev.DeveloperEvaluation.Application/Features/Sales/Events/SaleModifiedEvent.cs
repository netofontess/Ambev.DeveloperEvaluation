using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Events
{
    public class SaleModifiedEvent : INotification
    {
        public string SaleId { get; }

        public SaleModifiedEvent(string saleId)
        {
            SaleId = saleId;
        }
    }
}