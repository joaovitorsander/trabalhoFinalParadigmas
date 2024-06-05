using APITrabalhoFinal.Services.DTOs;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace APITrabalhoFinal.Services.Validate
{
    public class SaleValidate : AbstractValidator<SaleDTO>
    {
        public SaleValidate()
        {
            RuleFor(sale => sale.Productid)
                .GreaterThan(0).WithMessage("O ID do produto associado à venda é obrigatório.");

            RuleFor(sale => sale.Qty)
                .GreaterThan(0).WithMessage("A quantidade vendida deve ser maior que zero.");
        }
    }
}
