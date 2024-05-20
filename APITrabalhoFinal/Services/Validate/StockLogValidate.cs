using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;

namespace APITrabalhoFinal.Services.Validate
{
    public class StockLogValidate
    {
        public static bool Validate(StockLogDTO stockLogDTO)
        {
            if (stockLogDTO.Productid <= 0)
            {
                throw new InvalidEntityException("O ID do produto associado ao registro de estoque é obrigatório.");
            }

            if (stockLogDTO.Qty == 0)
            {
                throw new InvalidEntityException("A quantidade no registro de estoque deve ser diferente de zero.");
            }

            return true;
        }
    }
}
