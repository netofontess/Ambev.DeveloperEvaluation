using System;
using MediatR;
using OneOf;
using OneOf.Types;
using Ambev.DeveloperEvaluation.Application.DTOs;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Queries
{
    public class GetSaleItemsQuery : IRequest<OneOf<IEnumerable<SaleItemDTO>, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        public Guid SaleId { get; set; }
    }
}