using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Queries;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers
{
    public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public GetSaleByIdQueryHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
                if (sale == null)
                    return new NotFound();

                return new SaleDTO
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
                        TotalAmount = item.GetTotalAmount(),
                        UpdatedAt = item.UpdatedAt
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