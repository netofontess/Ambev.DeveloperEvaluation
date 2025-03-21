using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers
{
    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public CancelSaleCommandHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
                if (sale == null)
                    return new NotFound();

                sale.Cancel();

                await _saleRepository.UpdateAsync(sale, cancellationToken);
                await _saleRepository.SaveChangesAsync(cancellationToken);

                return new SaleDTO
                {
                    Id = sale.Id,
                    CustomerName = sale.CustomerName,
                    CustomerId = sale.CustomerId,
                    BranchName = sale.BranchName,
                    BranchId = sale.BranchId,
                    UserId = sale.UserId,
                    CreatedAt = sale.CreatedAt,
                    IsCancelled = sale.IsCancelled,
                    TotalAmount = sale.GetTotalAmount(),
                    Items = sale.Items.Select(item => new SaleItemDTO
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountPercentage = item.DiscountPercentage,
                        IsCancelled = item.IsCancelled,
                        TotalAmount = item.GetTotalAmount()
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new Ambev.DeveloperEvaluation.Application.Common.Error(ex.Message);
            }
        }
    }
}