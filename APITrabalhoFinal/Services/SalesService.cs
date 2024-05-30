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
        private readonly IValidator<SaleDTO> _validator;

        public SalesService(TfDbContext dbcontext, IValidator<SaleDTO> validator)
        {
            _dbContext = dbcontext;
            _validator = validator;
        }

        public TbSale Insert(SaleDTO dto)
        {
            ValidateSale(dto);

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


        private void ValidateSale(SaleDTO dto)
        {
            var validationResult = _validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new InvalidEntityException(errorMessages);
            }
        }
    }
}
