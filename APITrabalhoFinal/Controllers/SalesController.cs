using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using FluentValidation.Results;
using System.Linq;

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
        /// <response code="201">Indica que a venda foi inserida com sucesso.</response>
        /// <response code="400">Indica que houve um erro de validação nos dados da venda ou que o estoque é insuficiente.</response>
        /// <response code="404">Indica que o produto com o ID especificado não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPost()]
        [ProducesResponseType(typeof(TbSale), 201)]
        public IActionResult Insert([FromBody] List<SaleDTO> sale)
        {
            try
            {
                var validationResults = new List<FluentValidation.Results.ValidationResult>();

                foreach (var saleItem in sale)
                {
                    var validationResult = _validator.Validate(saleItem);
                    validationResults.Add(validationResult);
                }

                if (validationResults.Any(result => !result.IsValid))
                {
                    var errors = validationResults
                        .SelectMany(result => result.Errors)
                        .ToList();

                    return BadRequest(errors);
                }

                var entity = _service.Insert(sale);
                return Ok(entity);
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

        /// <summary>
        /// Obtém um relatório de vendas por período.
        /// </summary>
        /// <param name="startDate">A data de início do período.</param>
        /// <param name="endDate">A data de fim do período.</param>
        /// <returns>Uma lista de relatórios de vendas agrupados por código da venda.</returns>
        /// <response code="200">Indica que o relatório de vendas foi retornado com sucesso.</response>
        /// <response code="400">Indica que as datas de início e fim não foram fornecidas ou são inválidas.</response>
        /// <response code="404">Indica que não foram encontradas vendas no período especificado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("report")]
        public ActionResult<List<SalesReportDTO>> GetSalesReport(DateTime startDate, DateTime endDate)
        {

            if (startDate == default || endDate == default)
            {
                return BadRequest("As datas de início e fim são obrigatórias.");
            }

            try
            {
                var report = _service.GetSalesReportByPeriod(startDate, endDate);
                return Ok(report);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
