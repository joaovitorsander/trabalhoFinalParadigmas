using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;

namespace APITrabalhoFinal.Services.Parser
{
    public class PromotionParser
    {
        public static TbPromotion ToEntity(PromotionDTO dto)
        {
            return new TbPromotion
            {
                Id = dto.Id,
                Startdate = dto.Startdate,
                Enddate = dto.Enddate,
                Promotiontype = dto.Promotiontype,
                Productid = dto.Productid,
                Value = dto.Value
            };
        }
    }
}
