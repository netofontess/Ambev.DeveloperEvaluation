using System;
using Ambev.DeveloperEvaluation.Application.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Queries
{
    public class GetSaleByIdQuery : IRequest<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        public Guid Id { get; set; }

        public GetSaleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}