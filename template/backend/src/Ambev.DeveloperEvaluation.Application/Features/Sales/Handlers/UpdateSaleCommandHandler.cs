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
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public UpdateSaleCommandHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
                if (sale == null)
                    return new NotFound();

                sale.Update(
                    request.CustomerName,
                    request.BranchName,
                    request.CustomerId,
                    request.BranchId,
                    request.UserId);

                await _saleRepository.UpdateAsync(sale, cancellationToken);
                await _saleRepository.SaveChangesAsync(cancellationToken);

                return new SaleDTO
                {
                    Id = sale.Id,
                    CustomerName = sale.CustomerName,
                    BranchName = sale.BranchName,
                    CustomerId = sale.CustomerId,
                    BranchId = sale.BranchId,
                    UserId = sale.UserId,
                    CreatedAt = sale.CreatedAt,
                    IsCancelled = sale.IsCancelled,
                    Items = sale.Items.Select(i => new SaleItemDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountPercentage = i.DiscountPercentage,
                        TotalAmount = i.TotalAmount,
                        IsCancelled = i.IsCancelled
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