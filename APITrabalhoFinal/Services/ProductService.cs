using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class ProductService
    {
        private readonly TfDbContext _dbContext;
        private readonly StockLogService _stockLogService;

        public ProductService(TfDbContext dbContext, StockLogService stockLogService)
        {
            _dbContext = dbContext;
            _stockLogService = stockLogService;
        }

        public TbProduct Insert(ProductDTO dto)
        {
            var entity = ProductParser.ToEntity(dto);
            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO
            {
                Productid = entity.Id,
                Qty = entity.Stock,
                Createdat = DateTime.Now
            });

            return entity;
        }

        public TbProduct Update(ProductUpdateDTO dto, int id)
        {
            var existingEntity = GetById(id);

            int oldStock = existingEntity.Stock;

            existingEntity.Description = dto.Description;
            existingEntity.Barcode = dto.Barcode;
            existingEntity.Barcodetype = dto.Barcodetype;
            existingEntity.Price = dto.Price;
            existingEntity.Costprice = dto.Costprice;

            _dbContext.Update(existingEntity);
            _dbContext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO
            {
                Productid = existingEntity.Id,
                Qty = existingEntity.Stock - oldStock,
                Createdat = DateTime.Now
            });

            return existingEntity;
        }

        public TbProduct GetById(int id)
        {
            var existingEntity = _dbContext.TbProducts.FirstOrDefault(p => p.Id == id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }
            return existingEntity;
        }

        public IEnumerable<TbProduct> Get()
        {
            var existingEntities = _dbContext.TbProducts.ToList();
            if (existingEntities == null || existingEntities.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado.");
            }
            return existingEntities;
        }

        public TbProduct GetByBarCode(string barCode)
        {
            var lowerBarCode = barCode.ToLower();
            var entity = _dbContext.TbProducts.FirstOrDefault(p => p.Barcode.ToLower() == lowerBarCode);

            if (entity == null)
            {
                throw new NotFoundException("Nenhum registro encontrado.");
            }

            return entity;
        }

        public IEnumerable<TbProduct> GetByDesc(string desc)
        {
            var lowerDesc = desc.ToLower();
            var existingEntities = _dbContext.TbProducts
                .Where(p => p.Description.ToLower().Contains(lowerDesc))
                .ToList();

            if (existingEntities == null || existingEntities.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado.");
            }

            return existingEntities;
        }

        public void AjustarStock(int productId, int quantity)
        {
            var product = GetById(productId);
            if (product == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            if (quantity == 0)
            {
                throw new InvalidEntityException("A quantidade a atualizar deve ser diferente de zero.");
            }

            if (product.Stock + quantity < 0)
            {
                throw new InsufficientStockException("Estoque insuficiente para realizar a operação.");
            }

            int oldStock = product.Stock;
            product.Stock += quantity;

            _dbContext.Update(product);
            _dbContext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO
            {
                Productid = product.Id,
                Qty = product.Stock - oldStock,
                Createdat = DateTime.Now
            });
        }
    }
}
