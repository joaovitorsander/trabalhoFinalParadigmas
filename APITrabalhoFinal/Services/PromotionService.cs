﻿using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Validate;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public PromotionService(TfDbContext dbcontext, IMapper mapper)
        {
            _dbContext = dbcontext;
            _mapper = mapper;
        }

        public TbPromotion Insert(PromotionDTO dto)
        {
            var productExists = _dbContext.TbProducts.Any(p => p.Id == dto.Productid);
            if (!productExists)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var entity = _mapper.Map<TbPromotion>(dto);

            entity.Startdate = DateTime.SpecifyKind(dto.Startdate, DateTimeKind.Unspecified);
            entity.Enddate = DateTime.SpecifyKind(dto.Enddate, DateTimeKind.Unspecified);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbPromotion Update(PromotionDTO dto, int id)
        {

            var productExists = _dbContext.TbProducts.Any(p => p.Id == dto.Productid);
            if (!productExists)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var existingEntity = GetById(id);

            existingEntity.Startdate = DateTime.SpecifyKind(dto.Startdate, DateTimeKind.Unspecified);
            existingEntity.Enddate = DateTime.SpecifyKind(dto.Enddate, DateTimeKind.Unspecified); 
            existingEntity.Promotiontype = dto.Promotiontype;
            existingEntity.Productid = dto.Productid;
            existingEntity.Value = dto.Value;

            _dbContext.Update(existingEntity);
            _dbContext.SaveChanges();

            return existingEntity;  
        }

        public TbPromotion GetById(int id)
        {
            var existingEntity = _dbContext.TbPromotions.FirstOrDefault(c => c.Id == id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Promoção não encontrada.");
            }
            return existingEntity;
        }

        public IEnumerable<TbPromotion> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            var productExists = _dbContext.TbProducts.Any(p => p.Id == productId);
            if (!productExists)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

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

        public List<TbPromotion> GetActivePromotions(int productId)
        {
            return _dbContext.TbPromotions
                .Where(p => p.Productid == productId
                            && p.Startdate <= DateTime.Now
                            && p.Enddate >= DateTime.Now)
                .OrderBy(p => p.Promotiontype)
                .ToList();
        }
    }
}
