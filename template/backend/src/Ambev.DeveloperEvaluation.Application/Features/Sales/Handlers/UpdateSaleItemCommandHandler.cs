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
    public class UpdateSaleItemCommandHandler : IRequestHandler<UpdateSaleItemCommand, OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public UpdateSaleItemCommandHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(UpdateSaleItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
                if (sale == null)
                    return new NotFound();

                if (sale.IsCancelled)
                    return new Ambev.DeveloperEvaluation.Application.Common.Error("Cannot update items in a cancelled sale");

                var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
                if (item == null)
                    return new NotFound();

                sale.UpdateItem(
                    request.ItemId,
                    request.Quantity,
                    request.UnitPrice,
                    request.DiscountPercentage);

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
                    UpdatedAt = sale.UpdatedAt,
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
                        IsCancelled = i.IsCancelled,
                        UpdatedAt = i.UpdatedAt
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