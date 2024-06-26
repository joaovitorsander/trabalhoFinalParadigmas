﻿using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services;
using Microsoft.AspNetCore.Mvc;
using APITrabalhoFinal.DataBase.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using System.Linq;

namespace APITrabalhoFinal.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as promoções.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {

        public readonly PromotionService _service;
        

        public PromotionsController(PromotionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Insere uma nova promoção.
        /// </summary>
        /// <param name="promotion">A promoção a ser inserida.</param>
        /// <returns>A promoção inserida.</returns>
        /// <response code="201">Indica que a promoção foi inserida com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que o id do produto passado não existe.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPost()]
        [ProducesResponseType(typeof(TbPromotion), 201)]
        [ProducesResponseType(500)]
        public ActionResult<TbPromotion> Insert(PromotionDTO promotion)
        {
            try
            {
                var entity = _service.Insert(promotion);
                return CreatedAtAction(nameof(Insert), new { id = entity.Id }, entity);
            }
            catch (InvalidDataException ex)
            {
                var errors = ex.ValidationErrors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Dados inválidos", Errors = errors });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza uma promoção existente.
        /// </summary>
        /// <param name="id">O ID da promoção a ser atualizada.</param>
        /// <param name="dto">Os dados atualizados da promoção.</param>
        /// <returns>A promoção atualizada.</returns>
        /// <response code="200">Indica que a promoção foi atualizada com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que a promoção com o ID especificado não foi encontrada ou o ID do produto não existe.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPut("{id}")]
        public ActionResult<TbPromotion> Put(int id, PromotionDTO dto)
        {
            try
            {
                var entity = _service.Update(dto, id);
                return Ok(entity);
            }
            catch (InvalidDataException ex)
            {
                var errors = ex.ValidationErrors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Dados inválidos", Errors = errors });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Busca todas as promoções de um produto em um determinado período.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="startDate">Data de início do período.</param>
        /// <param name="endDate">Data de fim do período.</param>
        /// <returns>Lista de promoções.</returns>
        /// <response code="200">Indica que a busca foi realizada com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que o ID do produto não foi encontrado ou nenhuma promoção foi encontrada para o período especificado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("product/{productId}/period")]
        public ActionResult<IEnumerable<TbPromotion>> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var entity = _service.GetPromotionsByProductAndPeriod(productId, startDate, endDate);
                return Ok(entity);
            }
            catch (InvalidEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
