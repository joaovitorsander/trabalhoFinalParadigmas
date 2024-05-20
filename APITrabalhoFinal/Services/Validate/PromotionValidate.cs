using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;

namespace APITrabalhoFinal.Services.Validate
{
    public class PromotionValidate
    {
        public static bool Validate(PromotionDTO promotionDTO)
        {
            if (promotionDTO.Startdate > promotionDTO.Enddate)
            {
                throw new InvalidEntityException("A data de início da promoção não pode ser posterior ao término.");
            }

            if (promotionDTO.Promotiontype <= 0)
            {
                throw new InvalidEntityException("O tipo da promoção é obrigatório.");
            }

            if (promotionDTO.Productid <= 0)
            {
                throw new InvalidEntityException("O ID do produto associado à promoção é obrigatório.");
            }

            if (promotionDTO.Value <= 0)
            {
                throw new InvalidEntityException("O valor da promoção deve ser maior que zero.");
            }

            return true; 
        }
    }
}
