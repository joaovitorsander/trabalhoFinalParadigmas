using APITrabalhoFinal.Services.DTOs;
using FluentValidation;
using FluentValidation.Results;
using System;

namespace APITrabalhoFinal.Services.Validate
{
    public class SaleValidate : AbstractValidator<SaleDTO>
    {
        public SaleValidate()
        {
            RuleFor(sale => sale.Code)
                .NotEmpty().WithMessage("O código da venda é obrigatório.")
                .Must(IsGuid).WithMessage("O código da venda deve ser um GUID válido.");

            RuleFor(sale => sale.Productid)
                .GreaterThan(0).WithMessage("O ID do produto associado à venda é obrigatório.");

            RuleFor(sale => sale.Price)
                .GreaterThan(0).WithMessage("O preço da venda deve ser maior que zero.");

            RuleFor(sale => sale.Qty)
                .GreaterThan(0).WithMessage("A quantidade vendida deve ser maior que zero.");
        }

        private bool IsGuid(string code)
        {
            return Guid.TryParse(code, out _);
        }
    }
}
