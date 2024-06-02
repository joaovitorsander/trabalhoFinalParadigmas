using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using FluentValidation;
using System.Collections.Generic;
using System;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class SalesService
    {
        private readonly TfDbContext _dbContext;
        private readonly ProductService _productService;
        private readonly PromotionService _promotionService;
        private readonly StockLogService _stockLogService;

        public SalesService(TfDbContext dbcontext, ProductService productService, PromotionService promotionService, StockLogService stockLogService)
        {
            _dbContext = dbcontext;
            _productService = productService;
            _promotionService = promotionService;
            _stockLogService = stockLogService;
        }

        public TbSale Insert(SaleDTO dto)
        {
            var product = _productService.GetById(dto.Productid);
            if (product == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            if (product.Stock < dto.Qty)
            {
                throw new InsufficientStockException("Estoque insuficiente para o produto: " + product.Description);
            }

            var promotions = _promotionService.GetActivePromotions(dto.Productid);


            decimal unitPrice = product.Price;

            foreach (var promotion in promotions)
            {
                unitPrice = ApplyPromotion(unitPrice, promotion);
            }


            decimal totalPrice = unitPrice * dto.Qty;

            var entity = SaleParser.ToEntity(dto);
            entity.Price = totalPrice; 

            product.Stock -= dto.Qty;

            _dbContext.Update(product);
            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            _stockLogService.InsertStockLog(new StockLogDTO
            {
                Productid = entity.Productid,
                Qty = -entity.Qty,
                Createdat = DateTime.Now
            });

            return entity;
        }


        public TbSale GetByCode(string code)
        {
            var existingEntity = _dbContext.TbSales.FirstOrDefault(c => c.Code == code);
            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            return existingEntity;
        }

        public decimal ApplyPromotion(decimal price, TbPromotion promotion)
        {
            switch (promotion.Promotiontype)
            {
                case 0:
                    return price * (1 - promotion.Value / 100);
                case 1:
                    return price - promotion.Value;
                default:
                    return price;
            }
        }
    }
}
