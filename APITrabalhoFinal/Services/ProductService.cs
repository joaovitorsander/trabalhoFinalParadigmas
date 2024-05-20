using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using APITrabalhoFinal.Services.Validate;
using System.Collections.Generic;
using System.Linq;

namespace APITrabalhoFinal.Services
{
    public class ProductService
    {
        private readonly TfDbContext _dbContext;

        public ProductService(TfDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public TbProduct Insert(ProductDTO dto)
        {
            if (!ProductValidate.Validate(dto))
                return null;

            var entity = ProductParser.ToEntity(dto);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbProduct Update(ProductDTO dto, int id)
        {
            if (!ProductValidate.Validate(dto))
                return null;


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

        public TbProduct GetByBarCode(int barCode)
        {
            var entity = _dbContext.TbProducts.FirstOrDefault(p => int.Parse(p.Barcode) == barCode);
            if (entity == null)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }
            return entity;
        }
    }
}
