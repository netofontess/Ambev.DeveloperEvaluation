using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services
{
    /// <summary>
    /// Service responsible for applying business rules to sales.
    /// </summary>
    public class SaleService : ISaleService
    {
        public void ApplyBusinessRules(Sale sale)
        {
            foreach (var item in sale.Items)
            {
                if (item.Quantity > 20)
                    throw new InvalidOperationException($"Cannot sell more than 20 units of {item.ProductName}.");

                if (item.Quantity >= 10 && item.Quantity <= 20)
                {
                    item.Discount = item.Quantity * item.UnitPrice * 0.20m;
                }
                else if (item.Quantity >= 4 && item.Quantity < 10)
                {
                    item.Discount = item.Quantity * item.UnitPrice * 0.10m;
                }
                else
                {
                    item.Discount = 0;
                }

                item.Total = (item.Quantity * item.UnitPrice) - item.Discount;
            }

            sale.TotalAmount = sale.Items.Sum(i => i.Total);
        }
    }
}
