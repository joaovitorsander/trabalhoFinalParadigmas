using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services;
using Microsoft.AspNetCore.Mvc;
using APITrabalhoFinal.DataBase.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;

namespace APITrabalhoFinal.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as promoções.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {

        public readonly PromotionService _service;
        public readonly IValidator<PromotionDTO> _validator;

        public PromotionController(PromotionService service, IValidator<PromotionDTO> validator)
        {
            _service = service;
            _validator = validator;
        }

        /// <summary>
        /// Insere uma nova promoção.
        /// </summary>
        /// <param name="promotion">A promoção a ser inserida.</param>
        /// <returns>A promoção inserida.</returns>
        [HttpPost()]
        public ActionResult<TbPromotion> Insert(PromotionDTO promotion)
        {
            try
            {
                var validationResult = _validator.Validate(promotion);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var entity = _service.Insert(promotion);
                return Ok(entity);
            }
            catch (InvalidEntityException E)
            {
                return BadRequest(E.Message);
            }
        }

        /// <summary>
        /// Atualiza uma promoção existente.
        /// </summary>
        /// <param name="id">O ID da promoção a ser atualizada.</param>
        /// <param name="dto">Os dados atualizados da promoção.</param>
        /// <returns>A promoção atualizada.</returns>
        [HttpPut("{id}")]
        public ActionResult<TbPromotion> Put(int id, PromotionDTO dto)
        {
            try
            {
                var checkPromotionById = _service.GetById(id);
                if (checkPromotionById == null)
                {
                    return NotFound();
                }

                var validationResult = _validator.Validate(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var entity = _service.Update(dto, id);
                return Ok(entity);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        /// <summary>
        /// Busca todas as promoções de um produto em um determinado período.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="startDate">Data de início do período.</param>
        /// <param name="endDate">Data de fim do período.</param>
        /// <returns>Lista de promoções.</returns>
        [HttpGet("product/{productId}/period")]
        public ActionResult<IEnumerable<TbPromotion>> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entity = _service.GetPromotionsByProductAndPeriod(productId, startDate, endDate);
                return Ok(entity);
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (System.Exception e)
            {
                return new ObjectResult(new { error = e.Message })
                {
                    StatusCode = 500
                };
            }
        }

    }
}
