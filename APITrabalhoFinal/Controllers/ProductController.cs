using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services.Validate;
using Microsoft.AspNetCore.Mvc;

namespace APITrabalhoFinal.Controllers
{
    /// <summary>
    /// Controlador para gerenciar os produtos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        public readonly ProductService _service;
        public readonly ProductValidate _validator;

        public ProductController(ProductService service, ProductValidate validator)
        {
            _service = service;
            _validator = validator;
        }

        /// <summary>
        /// Insere um novo produto.
        /// </summary>
        /// <param name="product">O produto a ser inserido.</param>
        /// <returns>O produto inserido.</returns>
        [HttpPost()]
        public ActionResult<TbProduct> Insert(ProductDTO product)
        {
            try
            {
                var validationResult = _validator.ValidateProduct(product);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                var entity = _service.Insert(product);
                return Ok(entity);
            }
            catch (InvalidEntityException E)
            {
                return BadRequest(E.Message);
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">O ID do produto a ser atualizado.</param>
        /// <param name="dto">Os novos dados do produto.</param>
        /// <returns>O produto atualizado.</returns>
        [HttpPut("{id}")]
        public ActionResult<TbProduct> Update(int id, ProductDTO dto)
        {
            try
            {
                var validationResult = _validator.ValidateProduct(dto);
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
        /// Obtém um produto pelo código de barras.
        /// </summary>
        /// <param name="barCode">O código de barras do produto a ser obtido.</param>
        /// <returns>O produto solicitado.</returns>
        [HttpGet("barcode")]
        public ActionResult<TbProduct> GetByBarCode([FromQuery] string barCode)
        {
            try
            {
                var entity = _service.GetByBarCode(barCode);
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
        /// Obtém um produto pela descrição.
        /// </summary>
        /// <param name="description">A descrição do produto a ser obtido.</param>
        /// <returns>A lista de produtos que contém a descrição.</returns>
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
            catch (System.Exception e)
            {
                return new ObjectResult(new { error = e.Message })
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// Obtém todos os produtos.
        /// </summary>
        /// <returns>Uma lista de todos os produtos.</returns>
        [HttpGet()]
        public ActionResult<TbProduct> Get()
        {
            try
            {
                var entity = _service.Get();
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
