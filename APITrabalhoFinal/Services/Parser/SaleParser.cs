﻿using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;

namespace APITrabalhoFinal.Services.Parser
{
    public class SaleParser
    {
        public static TbSale ToEntity(SaleDTO dto)
        {
            return new TbSale
            {
                Id = dto.Id,
                Code = dto.Code,
                Createat = dto.Createat,
                Productid = dto.Productid,
                Price = dto.Price,
                Qty = dto.Qty,
                Discount = dto.Discount
            };
        }
    }
}
