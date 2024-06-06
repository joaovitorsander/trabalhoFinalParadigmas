using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class ProductService
    {
        private readonly TfDbContext _dbContext;
        private readonly StockLogService _stockLogService;
        private readonly IMapper _mapper;
        public readonly IValidator<ProductDTO> _validatorInsertProduct;
        public readonly IValidator<ProductUpdateDTO> _validatorUpdateProduct;

        public ProductService(TfDbContext dbContext, StockLogService stockLogService, IMapper mapper, IValidator<ProductDTO> validatorInsertProduct, IValidator<ProductUpdateDTO> validatorUpdateProduct)
        {
            _dbContext = dbContext;
            _stockLogService = stockLogService;
            _mapper = mapper;
            _validatorInsertProduct = validatorInsertProduct;
            _validatorUpdateProduct = validatorUpdateProduct;
        }

        public TbProduct Insert(ProductDTO dto)
        {
            var validationResult = _validatorInsertProduct.Validate(dto);
            if (!validationResult.IsValid)
            {
                throw new InvalidDataException("Dados inválidos", validationResult.Errors);
            }

            var entity = _mapper.Map<TbProduct>(dto);
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

            var validationResult = _validatorUpdateProduct.Validate(dto);
            if (!validationResult.IsValid)
            {
                throw new InvalidDataException("Dados inválidos", validationResult.Errors);
            }

            int oldStock = existingEntity.Stock;

            _mapper.Map(dto, existingEntity);

            _dbContext.Update(existingEntity);
            _dbContext.SaveChanges();

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

        public TbProduct GetByBarCode(string barCode)
        {
            if (string.IsNullOrWhiteSpace(barCode))
            {
                throw new InvalidEntityException("O código de barras não pode ser vazio ou nulo.");
            }

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
                throw new InvalidDataException("A quantidade a atualizar deve ser diferente de zero.", null);
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
