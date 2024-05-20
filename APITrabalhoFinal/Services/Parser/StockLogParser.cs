using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;

namespace APITrabalhoFinal.Services.Parser
{
    public class StockLogParser
    {
        public static TbStockLog ToEntity(StockLogDTO dto)
        {
            return new TbStockLog
            {
                Id = dto.Id,
                Productid = dto.Productid,
                Qty = dto.Qty,
                Createdat = dto.Createdat
            };
        }
    }
}
