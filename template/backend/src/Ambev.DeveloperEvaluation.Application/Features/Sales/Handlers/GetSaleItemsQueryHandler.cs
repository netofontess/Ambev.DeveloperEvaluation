using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Queries;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers
{
    public class GetSaleItemsQueryHandler : IRequestHandler<GetSaleItemsQuery, OneOf<IEnumerable<SaleItemDTO>, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public GetSaleItemsQueryHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<IEnumerable<SaleItemDTO>, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(GetSaleItemsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
                if (sale == null)
                    return new NotFound();

                return sale.Items.Select(item => new SaleItemDTO
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercentage = item.DiscountPercentage,
                    IsCancelled = item.IsCancelled,
                    TotalAmount = item.GetTotalAmount(),
                    UpdatedAt = item.UpdatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                return new Ambev.DeveloperEvaluation.Application.Common.Error(ex.Message);
            }
        }
    }
}