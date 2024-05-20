using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;

namespace APITrabalhoFinal.Services.Validate
{
    public class ProductValidate
    {
        public static bool Validate(ProductDTO productDTO)
        {
            if (string.IsNullOrEmpty(productDTO.Description))
            {
                throw new InvalidEntityException("A descrição do produto é obrigatória.");
            }

            if (productDTO.Description.Length > 255)
            {
                throw new InvalidEntityException("A descrição do produto não pode exceder 255 caracteres.");
            }

            if (string.IsNullOrEmpty(productDTO.Barcode))
            {
                throw new InvalidEntityException("O código de barras do produto é obrigatório.");
            }

            if (productDTO.Barcode.Length > 40)
            {
                throw new InvalidEntityException("O código de barras do produto não pode exceder 40 caracteres.");
            }

            if (string.IsNullOrEmpty(productDTO.Barcodetype))
            {
                throw new InvalidEntityException("O tipo de código de barras do produto é obrigatório.");
            }

            if (productDTO.Barcodetype.Length > 40)
            {
                throw new InvalidEntityException("O tipo de código de barras do produto não pode exceder 40 caracteres.");
            }

            if (productDTO.Stock <= 0)
            {
                throw new InvalidEntityException("A quantidade em estoque deve ser maior que zero.");
            }

            if (productDTO.Price <= 0)
            {
                throw new InvalidEntityException("O preço do produto deve ser maior que zero.");
            }

            if (productDTO.Costprice <= 0)
            {
                throw new InvalidEntityException("O preço de custo do produto deve ser maior que zero.");
            }

            return true;
        }
    }
}
