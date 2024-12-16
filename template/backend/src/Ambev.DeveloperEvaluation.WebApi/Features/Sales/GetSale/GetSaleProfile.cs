using Ambev.DeveloperEvaluation.Application.GetSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    public class GetSaleProfile : Profile
    {
        public GetSaleProfile()
        {
            CreateMap<GetSaleResult, GetSaleResponse>();
            CreateMap<SaleItemResult, GetSaleItemResponse>();
        }
    }
}
