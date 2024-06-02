using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;

namespace APITrabalhoFinal.Services.Parser
{
    public class SaleParser
    {
        public static TbSale ToEntity(SaleDTO dto)
        {
            return new TbSale
            {
                Code = dto.Code,
                Createat = dto.Createat,
                Productid = dto.Productid,
                Qty = dto.Qty,
                Discount = dto.Discount
            };
        }
    }
}
