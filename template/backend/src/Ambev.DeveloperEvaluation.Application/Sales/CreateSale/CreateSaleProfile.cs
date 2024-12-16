using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Initializes the mappings for CreateSale operation.
    /// </summary>
    public class CreateSaleProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSaleProfile"/> class.
        /// </summary>
        public CreateSaleProfile()
        {
            // Mapping from command to domain entity
            CreateMap<CreateSaleCommand, Sale>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // Mapping from sale item command to sale item entity
            CreateMap<CreateSaleItemCommand, SaleItem>();

            // Mapping from domain entity to result object
            CreateMap<Sale, CreateSaleResult>()
                .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.IsSuccessful, opt => opt.MapFrom(_ => true));
        }
    }
}
