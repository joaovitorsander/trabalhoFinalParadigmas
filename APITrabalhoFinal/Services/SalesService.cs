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

        public SalesService(TfDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public TbSale Insert(SaleDTO dto)
        {
            var entity = SaleParser.ToEntity(dto);

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
    }
}
