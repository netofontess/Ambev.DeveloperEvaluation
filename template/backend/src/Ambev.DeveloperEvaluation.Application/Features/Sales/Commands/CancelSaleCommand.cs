using System;
using MediatR;
using OneOf;
using OneOf.Types;
using Ambev.DeveloperEvaluation.Application.DTOs;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Commands
{
    public class CancelSaleCommand : IRequest<OneOf<SaleDTO, NotFound, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        public Guid Id { get; set; }
    }
}