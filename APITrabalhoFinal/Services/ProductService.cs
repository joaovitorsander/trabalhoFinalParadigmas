using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
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

            //AjustarStock(entity.Id, entity.Stock);

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

            AjustarStock(entity.Id, entity.Stock);

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

            var existingEntity = _dbContext.TbProducts
                .Where(p => p.Description.ToLower().Contains(lowerDesc))
                .ToList();

            if (existingEntity == null || existingEntity.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }

            return existingEntity;
        }

        public void AjustarStock(int productId, int quantity)
        {
            var product = GetById(productId);
            if (product == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            if (quantity > 0)
            {
                product.Stock += quantity;
            }
            else if (quantity < 0)
            {
                if (product.Stock < Math.Abs(quantity))
                {
                    throw new InsufficientStockException("Estoque insuficiente para realizar a operação.");
                }

                product.Stock += quantity;
            }
            else
            {
                return;
            }

            var stockLog = new TbStockLog
            {
                Productid = productId,
                Qty = quantity,
                Createdat = DateTime.Now
            };

            _dbContext.TbStockLogs.Add(stockLog);
            _dbContext.SaveChanges();
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
