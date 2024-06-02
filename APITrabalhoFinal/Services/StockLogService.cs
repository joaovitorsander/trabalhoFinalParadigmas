using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using System.Collections.Generic;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class StockLogService
    {
        private readonly TfDbContext _dbContext;

        public StockLogService(TfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TbStockLog InsertStockLog(StockLogDTO dto)
        {
            var entity = StockLogParser.ToEntity(dto);
            _dbContext.TbStockLogs.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public List<StockLogResultDTO> GetStockLogByProductId(int productId)
        {
            var productExists = _dbContext.TbProducts.Any(p => p.Id == productId);

            if (!productExists)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var logs = from log in _dbContext.TbStockLogs
                       where log.Productid == productId
                       select new StockLogResultDTO
                       {
                           Date = log.Createdat,
                           Barcode = log.Product.Barcode,
                           Description = log.Product.Description,
                           Quantity = log.Qty
                       };

            if (!logs.Any())
            {
                throw new NotFoundException("Nenhum log encontrado para o produto.");
            }

            return logs.ToList();
        }
    }
}
