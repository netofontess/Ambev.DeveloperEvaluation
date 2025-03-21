using System;
using System.Collections.Generic;
using MediatR;
using OneOf;
using Ambev.DeveloperEvaluation.Application.DTOs;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Commands
{
    public class CreateSaleCommand : IRequest<OneOf<SaleDTO, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        public string CustomerName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public List<CreateSaleItemCommand> Items { get; set; } = new();
    }

    public class CreateSaleItemCommand
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}