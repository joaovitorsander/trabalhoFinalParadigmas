using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace APITrabalhoFinal.Controllers
{
    /// <summary>
    /// Controlador para gerenciar os produtos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        public readonly ProductService _service;
    
        public ProductsController(ProductService service) 
        { 
            _service = service;
        }

        /// <summary>
        /// Insere um novo produto.
        /// </summary>
        /// <param name="product">O produto a ser inserido.</param>
        /// <returns>O produto inserido.</returns>
        /// <response code="201">Indica que o produto foi inserido com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPost()]
        [ProducesResponseType(typeof(TbProduct), 201)]
        [ProducesResponseType(500)]
        public ActionResult<TbProduct> Insert(ProductDTO product)
        {
            try
            {
                var entity = _service.Insert(product);
                return CreatedAtAction(nameof(Insert), new { id = entity.Id }, entity);
            }
            catch (InvalidDataException ex)
            {
                var errors = ex.ValidationErrors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Dados inválidos", Errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">O ID do produto a ser atualizado.</param>
        /// <param name="dto">Os novos dados do produto.</param>
        /// <returns>O produto atualizado.</returns>
        /// <response code="200">Indica que o produto foi atualizado com sucesso.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que o produto com o ID especificado não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpPut("{id}")]
        public ActionResult<TbProduct> Update(int id, ProductUpdateDTO dto)
        {
            try
            {
                var entity = _service.Update(dto, id);
                return Ok(entity);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Obtém um produto pelo código de barras.
        /// </summary>
        /// <param name="barCode">O código de barras do produto a ser obtido.</param>
        /// <returns>O produto solicitado.</returns>
        /// <response code="200">Indica que a operação foi bem-sucedida e retorna o produto correspondente ao código de barras.</response>
        /// <response code="400">Indica que os dados fornecidos são inválidos.</response>
        /// <response code="404">Indica que nenhum produto correspondente ao código de barras foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("barcode")]
        public ActionResult<TbProduct> GetByBarCode([FromQuery] string barCode)
        {
            try
            {
                var entity = _service.GetByBarCode(barCode);
                return Ok(entity);
            }
            catch (InvalidEntityException E) 
            { 
                return BadRequest(E.Message); 
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Obtém um produto pela descrição.
        /// </summary>
        /// <param name="description">A descrição do produto a ser obtido.</param>
        /// <returns>A lista de produtos que contém a descrição.</returns>
        /// <response code="200">Indica que a operação foi bem-sucedida e retorna os produtos que correspondem à descrição.</response>
        /// <response code="404">Indica que nenhum produto correspondente à descrição foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("description/{description}")]
        public ActionResult<TbProduct> GetByDesc(string description)
        {
            try
            {
                var entity = _service.GetByDesc(description);
                return Ok(entity);
            }
            catch (NotFoundException E)
            {
                return NotFound(E.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza o estoque de um produto pelo ID.
        /// </summary>
        /// <remarks>
        /// O método permite aumentar ou diminuir o estoque de um produto existente especificado pelo seu ID.
        /// </remarks>
        /// <param name="id">O ID do produto a ser atualizado o estoque.</param>
        /// <param name="stockUpdate">Os dados de atualização do estoque, contendo a quantidade a ser adicionada ou removida.</param>
        /// <returns>Um código de status HTTP que indica o resultado da operação.</returns>
        /// <response code="200">Indica que o estoque do produto foi atualizado com sucesso.</response>
        /// <response code="400">Indica que os dados são inválidos.</response>
        /// <response code="404">Indica que o produto especificado pelo ID não foi encontrado.</response>
        /// <response code="500">Indica que ocorreu um erro interno ao tentar atualizar o estoque.</response>
        [HttpPut("{id}/stock")]
        public IActionResult UpdateStock(int id, [FromBody] StockUpdateDTO stockUpdate)
        {
            try
            {
                _service.AjustarStock(id, stockUpdate.Quantity);
                return Ok("Estoque atualizado com sucesso.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficientStockException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
