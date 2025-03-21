using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Queries;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using OneOf;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers
{
    public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, OneOf<IEnumerable<SaleDTO>, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public GetSalesQueryHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<IEnumerable<SaleDTO>, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sales = await _saleRepository.GetSalesAsync(
                    request.Page,
                    request.Size,
                    request.StartDate,
                    request.EndDate,
                    request.CustomerName,
                    request.CustomerId,
                    request.BranchId,
                    request.IsCancelled,
                    request.MinAmount,
                    request.MaxAmount,
                    request.OrderBy,
                    cancellationToken);

                var result = sales.Select(sale => new SaleDTO
                {
                    Id = sale.Id,
                    CustomerName = sale.CustomerName,
                    CustomerId = sale.CustomerId,
                    BranchName = sale.BranchName,
                    BranchId = sale.BranchId,
                    UserId = sale.UserId,
                    CreatedAt = sale.CreatedAt,
                    UpdatedAt = sale.UpdatedAt,
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
                        TotalAmount = item.TotalAmount,
                        UpdatedAt = item.UpdatedAt
                    }).ToList()
                });

                return OneOf<IEnumerable<SaleDTO>, Ambev.DeveloperEvaluation.Application.Common.Error>.FromT0(result);
            }
            catch (Exception ex)
            {
                return new Ambev.DeveloperEvaluation.Application.Common.Error($"Error retrieving sales: {ex.Message}");
            }
        }
    }
}