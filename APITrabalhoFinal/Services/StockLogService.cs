using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class StockLogService
    {
        private readonly TfDbContext _dbContext;
        private readonly IMapper _mapper;

        public StockLogService(TfDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public TbStockLog InsertStockLog(StockLogDTO dto)
        {
            var entity = _mapper.Map<TbStockLog>(dto);
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
