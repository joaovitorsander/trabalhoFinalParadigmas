using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class PromotionService
    {
        private readonly TfDbContext _dbContext;

        public PromotionService(TfDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public TbPromotion Insert(PromotionDTO dto)
        {

            var entity = PromotionParser.ToEntity(dto);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbPromotion Update(PromotionDTO dto, int id)
        {

            var promotion = GetById(id);

            if (promotion == null)
            {
                throw new NotFoundException("Registro não existe");
            }

            var entity = PromotionParser.ToEntity(dto);

            promotion.Startdate = dto.Startdate;
            promotion.Enddate = dto.Enddate;
            promotion.Promotiontype = dto.Promotiontype;
            promotion.Productid = dto.Productid;
            promotion.Value = dto.Value;

            _dbContext.Update(promotion);
            _dbContext.SaveChanges();

            return entity;
        }



        public TbPromotion GetById(int id)
        {
            var existingEntity = _dbContext.TbPromotions.FirstOrDefault(c => c.Id == id);
            if (existingEntity == null)
            {
                return null;
            }
            return existingEntity;
        }

        public IEnumerable<TbPromotion> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            var promotions = _dbContext.TbPromotions
                .Where(p => p.Productid == productId &&
                            p.Startdate >= startDate &&
                            p.Enddate <= endDate)
                .ToList();

            if (promotions == null || promotions.Count == 0)
            {
                throw new NotFoundException("Nenhuma promoção encontrada para o período especificado.");
            }

            return promotions;
        }
    }
}
