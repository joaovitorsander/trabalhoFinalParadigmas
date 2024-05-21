using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Parser;
using APITrabalhoFinal.Services.Validate;
using System.Collections.Generic;
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
            if (!PromotionValidate.Validate(dto))
                return null;

            var entity = PromotionParser.ToEntity(dto);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbPromotion Update(PromotionDTO dto, int id)
        {
            if (!PromotionValidate.Validate(dto))
                return null;


            var existingEntity = GetById(id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }

            var entity = PromotionParser.ToEntity(dto);

            var ProductById = GetById(id);

            ProductById.Startdate = entity.Startdate;
            ProductById.Enddate = entity.Enddate;
            ProductById.Promotiontype = entity.Promotiontype;
            ProductById.Value = entity.Value;


            _dbContext.Update(ProductById);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbPromotion GetById(int id)
        {
            var existingEntity = _dbContext.TbPromotions.FirstOrDefault(c => c.Id == id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            return existingEntity;
        }
        public IEnumerable<TbPromotion> Get()
        {
            var existingEntity = _dbContext.TbPromotions.ToList();
            if (existingEntity == null || existingEntity.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }
            return existingEntity;
        }
        public void Delete(int id)
        {
            var existingEntity = GetById(id);

            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            _dbContext.Remove(existingEntity);
            _dbContext.SaveChanges();

        }
    }
}
