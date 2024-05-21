using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using APITrabalhoFinal.Services.Validate;
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
        private readonly IValidator<ProductDTO> _validator;

        public ProductService(TfDbContext dbcontext, IValidator<ProductDTO> validator)
        {
            _dbContext = dbcontext;
            _validator = validator;
        }


        public TbProduct Insert(ProductDTO dto)
        {
            ValidateProduct(dto);

            var entity = ProductParser.ToEntity(dto);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbProduct Update(ProductDTO dto, int id)
        {
            ValidateProduct(dto);

            var existingEntity = GetById(id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }

            var entity = ProductParser.ToEntity(dto);

            var ProductById = GetById(id);

            ProductById.Description = entity.Description;
            ProductById.Barcode = entity.Barcode;
            ProductById.Barcodetype = entity.Barcodetype;
            ProductById.Price = entity.Price;
            ProductById.Costprice = entity.Costprice;


            _dbContext.Update(ProductById);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbProduct GetById(int id)
        {
            var existingEntity = _dbContext.TbProducts.FirstOrDefault(p => p.Id == id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            return existingEntity;
        }
        public IEnumerable<TbProduct> Get()
        {
            var existingEntity = _dbContext.TbProducts.ToList();
            if (existingEntity == null || existingEntity.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }
            return existingEntity;
        }

        public TbProduct GetByBarCode(string barCode)
        {
            var lowerBarCode = barCode.ToLower();

            var entity = _dbContext.TbProducts.FirstOrDefault(p => p.Barcode.ToLower() == lowerBarCode);

            if (entity == null)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }

            return entity;
        }

        public IEnumerable<TbProduct> GetByDesc(string desc)
        {
            var lowerDesc = desc.ToLower(); 

            var entities = _dbContext.TbProducts
                .Where(p => p.Description.ToLower().Contains(lowerDesc))
                .ToList();

            if (entities.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }

            return entities;
        }


        private void ValidateProduct(ProductDTO dto)
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
