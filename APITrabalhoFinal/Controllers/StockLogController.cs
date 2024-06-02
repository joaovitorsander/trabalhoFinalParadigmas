using APITrabalhoFinal.Services;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace APITrabalhoFinal.Controllers
{
    public class StockLogController : ControllerBase
    {
        public readonly StockLogService _service;

        public StockLogController(StockLogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtém os logs de um determinado produto.
        /// </summary>
        /// <param name="productId">O ID do produto a ser obtido os logs.</param>
        /// <returns>A lista de logs do produto.</returns>
        /// <response code="200">Indica que a operação foi bem-sucedida e retorna logs correspondentes ao produto.</response>
        /// <response code="404">Indica que o ID do produto informado não existe ou nenhum log foi encontrado para o mesmo.</response>
        /// <response code="500">Indica que ocorreu um erro interno no servidor.</response>
        [HttpGet("{productId}/logs")]
        public ActionResult<List<StockLogResultDTO>> GetStockLogs(int productId)
        {
            try
            {
                var logs = _service.GetStockLogByProductId(productId);
                return logs;
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
    }
}
