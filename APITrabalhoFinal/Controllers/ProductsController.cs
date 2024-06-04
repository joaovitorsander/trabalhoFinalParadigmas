using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Validate;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public readonly IValidator<ProductDTO> _validatorInsertProduct;
        public readonly IValidator<ProductUpdateDTO> _validatorUpdateProduct;


        public ProductsController(ProductService service, IValidator<ProductDTO> validatorInsertProduct, IValidator<ProductUpdateDTO> validatorUpdateProduct)
        {
            _service = service;
            _validatorInsertProduct = validatorInsertProduct;
            _validatorUpdateProduct = validatorUpdateProduct;
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
        public ActionResult<TbProduct> Insert(ProductDTO product)
        {
            try
            {
                var validationResult = _validatorInsertProduct.Validate(product);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Message = "Dados inválidos", Errors = validationResult.Errors });
                }
                var entity = _service.Insert(product);
                return Ok(entity);
            }
            catch (InvalidEntityException E)
            {
                return BadRequest(E.Message);
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
                var checkProductById = _service.GetById(id);
                if (checkProductById == null)
                {
                    return NotFound("Produto com o ID especificado não foi encontrado");
                }

                var validationResult = _validatorUpdateProduct.Validate(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Message = "Dados inválidos", Errors = validationResult.Errors });
                }

                var entity = _service.Update(dto, id);
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
                if (string.IsNullOrWhiteSpace(barCode))
                {
                    return BadRequest("O código de barras não pode ser vazio ou nulo.");
                }

                var entity = _service.GetByBarCode(barCode);
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
                var product = _service.GetById(id);
                if (product == null)
                {
                    return NotFound("Produto não encontrado.");
                }

                if (stockUpdate.Quantity == 0)
                {
                    return BadRequest("A quantidade a atualizar deve ser diferente de zero.");
                }

                _service.AjustarStock(id, stockUpdate.Quantity);

                return Ok("Estoque atualizado com sucesso.");
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
    }
}
