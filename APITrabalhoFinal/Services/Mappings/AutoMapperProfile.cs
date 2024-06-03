using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using AutoMapper;

namespace APITrabalhoFinal.Services.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product Mappings
            CreateMap<ProductDTO, TbProduct>();
            CreateMap<TbProduct, ProductDTO>();
            CreateMap<ProductUpdateDTO, TbProduct>();

            // Stock Log Mappings
            CreateMap<StockLogDTO, TbStockLog>();
            CreateMap<TbStockLog, StockLogDTO>();

            // Promotion Mappings
            CreateMap<PromotionDTO, TbPromotion>();
            CreateMap<TbPromotion, PromotionDTO>();

            // Sale Mappings
            CreateMap<SaleDTO, TbSale>();
            CreateMap<TbSale, SaleDTO>();
        }
    }
}
