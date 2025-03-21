using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.DTOs;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// AutoMapper profile for sale retrieval mappings
/// </summary>
public sealed class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleProfile"/> class
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<SaleDTO, GetSaleResponse>();
        CreateMap<SaleItemDTO, SaleItemResponse>();
    }
}