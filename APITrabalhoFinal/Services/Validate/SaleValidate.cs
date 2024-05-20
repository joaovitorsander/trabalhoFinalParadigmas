using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;

namespace APITrabalhoFinal.Services.Validate
{
    public class SaleValidate
    {
        public static bool Validate(SaleDTO saleDTO)
        {
            if (string.IsNullOrEmpty(saleDTO.Code))
            {
                throw new InvalidEntityException("O código da venda é obrigatório.");
            }

            if (saleDTO.Productid <= 0)
            {
                throw new InvalidEntityException("O ID do produto associado à venda é obrigatório.");
            }

            if (saleDTO.Price <= 0)
            {
                throw new InvalidEntityException("O preço da venda deve ser maior que zero.");
            }

            if (saleDTO.Qty <= 0)
            {
                throw new InvalidEntityException("A quantidade vendida deve ser maior que zero.");
            }

            if (saleDTO.Discount < 0 || saleDTO.Discount > 100)
            {
                throw new InvalidEntityException("O desconto deve estar entre 0% e 100%.");
            }

            return true; 
        }
    }
}
