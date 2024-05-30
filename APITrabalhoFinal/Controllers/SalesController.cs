using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
            catch (InvalidEntityException E)
            {
                return BadRequest(E.Message);
            }
        }

        /// <summary>
        /// Obtém uma venda pelo código.
        /// </summary>
        /// <param name="cod">O código da venda a ser obtida.</param>
        /// <returns>A venda solicitada.</returns>
        [HttpGet("{id}")]
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
