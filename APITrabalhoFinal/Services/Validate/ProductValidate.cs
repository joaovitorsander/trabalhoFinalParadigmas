using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using FluentValidation;

namespace APITrabalhoFinal.Services.Validate
{
    public class ProductValidate : AbstractValidator<ProductDTO>
    {
        public ProductValidate()
        {
            RuleFor(product => product.Description)
                .NotEmpty().WithMessage("A descrição do produto é obrigatória.")
                .MaximumLength(255).WithMessage("A descrição do produto não pode exceder 255 caracteres.");

            RuleFor(product => product.Barcode)
                .NotEmpty().WithMessage("O código de barras do produto é obrigatório.")
                .MaximumLength(40).WithMessage("O código de barras do produto não pode exceder 40 caracteres.");

            RuleFor(product => product.Barcodetype)
                .NotEmpty().WithMessage("O tipo de código de barras do produto é obrigatório.")
                .MaximumLength(10).WithMessage("O tipo de código de barras do produto não pode exceder 10 caracteres.");

            RuleFor(product => product.Stock)
                .GreaterThan(0).WithMessage("A quantidade em estoque deve ser maior que zero.");

            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");

            RuleFor(product => product.Costprice)
                .GreaterThan(0).WithMessage("O preço de custo do produto deve ser maior que zero.");
        }
    }
}
