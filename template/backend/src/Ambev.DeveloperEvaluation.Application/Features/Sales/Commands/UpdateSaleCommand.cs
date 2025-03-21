using System;
using MediatR;
using OneOf;
using OneOf.Types;
using Ambev.DeveloperEvaluation.Application.DTOs;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Commands
{
    public class UpdateSaleCommand : IRequest<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerId { get; set; }
        public string BranchName { get; set; }
        public Guid BranchId { get; set; }
        public Guid UserId { get; set; }
    }
}