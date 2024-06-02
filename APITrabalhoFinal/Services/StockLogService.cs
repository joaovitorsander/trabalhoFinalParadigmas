using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Parser;

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
    }
}
