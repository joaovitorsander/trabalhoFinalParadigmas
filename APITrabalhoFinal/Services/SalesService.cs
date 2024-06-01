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

        public SalesService(TfDbContext dbcontext, ProductService productService, PromotionService promotionService)
        {
            _dbContext = dbcontext;
            _productService = productService;
            _promotionService = promotionService;
        }

        public TbSale Insert(SaleDTO dto)
        {
            var entity = SaleParser.ToEntity(dto);

            var product = _productService.GetById(entity.Productid);
            if (product == null)
            {
                throw new InvalidEntityException("Produto não encontrado");
            }
            if (product.Stock < entity.Qty)
            {
                throw new InsufficientStockException("Estoque insuficiente para o produto: " + product.Description);
            }
            product.Stock -= entity.Qty;

            var promotions = _promotionService.GetActivePromotions(entity.Productid);

            foreach (var promotion in promotions)
            {
                entity.Price = ApplyPromotion(entity.Price, promotion);
            }

            _dbContext.Update(product);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

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
