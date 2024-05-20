using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using APITrabalhoFinal.Services;
using Microsoft.AspNetCore.Mvc;
using APITrabalhoFinal.DataBase.Models;

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

        public PromotionController(PromotionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Insere uma nova promoção.
        /// </summary>
        /// <param name="promoção">A promoção a ser inserida.</param>
        /// <returns>A promoção inserida.</returns>
        [HttpPost()]
        public ActionResult<TbPromotion> Insert(PromotionDTO promotion)
        {
            try
            {
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
        /// <param name="dto">Os novos dados da promoção.</param>
        /// <returns>A promoção atualizada.</returns>
        [HttpPut("{id}")]
        public ActionResult<TbPromotion> Update(int id, PromotionDTO dto)
        {
            try
            {
                var entity = _service.Update(dto, id);
                return Ok(entity);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Exclui uma promoção.
        /// </summary>
        /// <param name="id">O ID da promoção a ser excluída.</param>
        /// <returns>Retorna NoContent se a exclusão for bem-sucedida.</returns>
        [HttpDelete("{id}")]
        public ActionResult<TbPromotion> Delete(int id)
        {
            try
            {
                _service.Delete(id);
                return NoContent();
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
        /// Obtém uma promoção pelo ID.
        /// </summary>
        /// <param name="id">O ID da promoção a ser obtida.</param>
        /// <returns>A promoção solicitada.</returns>
        [HttpGet("{id}")]
        public ActionResult<TbPromotion> GetById(int id)
        {
            try
            {
                var entity = _service.GetById(id);
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
        /// Obtém todas as promoções.
        /// </summary>
        /// <returns>Uma lista de todas as promoções.</returns>
        [HttpGet()]
        public ActionResult<TbPromotion> Get()
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
