using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Ambev.DeveloperEvaluation.Application.Features.Sales.Queries
{
    public class GetSalesQuery : IRequest<OneOf<IEnumerable<SaleDTO>, Ambev.DeveloperEvaluation.Application.Common.Error>>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CustomerName { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BranchId { get; set; }
        public bool? IsCancelled { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string OrderBy { get; set; }
    }
}