using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Features.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Interfaces.Repositories;
using MediatR;
using OneOf;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Handlers
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, OneOf<SaleDTO, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        private readonly ISaleRepository _saleRepository;

        public CreateSaleCommandHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<OneOf<SaleDTO, Ambev.DeveloperEvaluation.Application.Common.Error>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = new Domain.Entities.Sale(
                    request.CustomerName,
                    request.BranchName,
                    request.CustomerId,
                    request.BranchId,
                    request.UserId);

                // Add items to the sale
                foreach (var item in request.Items)
                {
                    sale.AddItem(
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.UnitPrice,
                        item.DiscountPercentage);
                }

                await _saleRepository.AddAsync(sale, cancellationToken);
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
                        TotalAmount = item.TotalAmount
                    }).ToList()
                };
            }
            catch (System.Exception ex)
            {
                return new Ambev.DeveloperEvaluation.Application.Common.Error($"Error creating sale: {ex.Message}");
            }
        }
    }
}