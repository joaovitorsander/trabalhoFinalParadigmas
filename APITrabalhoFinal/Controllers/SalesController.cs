using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;

namespace APITrabalhoFinal.Controllers
{
    /// <summary>
    /// Controlador para gerenciar as vendas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        public readonly SalesService _service;
        public readonly IValidator<SaleDTO> _validator;

        public SalesController(SalesService service, IValidator<SaleDTO> validator)
        {
            _service = service;
            _validator = validator;
        }

        /// <summary>
        /// Insere uma nova venda.
        /// </summary>
        /// <param name="sale">A venda a ser inserida</param>
        /// <returns>A venda inserida.</returns>
        /// <response code="200">Indica que a venda foi inserida com sucesso.</response>
        /// <response code="400">Indica que houve um erro de validação nos dados da venda ou que o estoque é insuficiente.</response>
        /// <response code="404">Indica que o produto com o ID especificado não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPost()]
        public ActionResult<TbSale> Insert(SaleDTO sale)
        {
            try
            {
                var validationResult = _validator.Validate(sale);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var entity = _service.Insert(sale);
                return Ok(entity);
            }
            catch (InvalidEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtém uma venda pelo código.
        /// </summary>
        /// <param name="cod">O código da venda a ser obtida.</param>
        /// <returns>A venda solicitada.</returns>
        /// <response code="200">Indica que a venda foi retornada com sucesso.</response>
        /// <response code="404">Indica que a venda com o código especificado não foi encontrada.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("{cod}")]
        public ActionResult<TbSale> GetByCode(string cod)
        {
            try
            {
                var entity = _service.GetByCode(cod);
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
