using APITrabalhoFinal.DataBase.Models;
using APITrabalhoFinal.Services.DTOs;
using APITrabalhoFinal.Services.Exceptions;
using FluentValidation;
using System.Collections.Generic;
using System;
using System.Linq;
using AutoMapper;

namespace APITrabalhoFinal.Services
{
    public class SalesService
    {
        private readonly TfDbContext _dbContext;
        private readonly ProductService _productService;
        private readonly PromotionService _promotionService;
        private readonly StockLogService _stockLogService;
        private readonly IMapper _mapper;
        public readonly IValidator<SaleDTO> _validator;

        public SalesService(TfDbContext dbcontext, ProductService productService, PromotionService promotionService, StockLogService stockLogService, IMapper mapper, IValidator<SaleDTO> validator)
        {
            _dbContext = dbcontext;
            _productService = productService;
            _promotionService = promotionService;
            _stockLogService = stockLogService;
            _mapper = mapper;
            _validator = validator;
        }

        public IEnumerable<TbSale> Insert(List<SaleDTO> dtoList)
        {
            var sales = new List<TbSale>();
            var currentTime = DateTime.Now;
            var code = Guid.NewGuid().ToString();

            foreach (var dto in dtoList)
            {
                var validationResult = _validator.Validate(dto);
                if (!validationResult.IsValid)
                {
                    throw new InvalidDataException("Dados inválidos", validationResult.Errors);
                }

                var product = _productService.GetById(dto.Productid);
                if (product == null)
                    throw new NotFoundException("Produto não existe");

                if (product.Stock < dto.Qty)
                    throw new InsufficientStockException("Estoque insuficiente para a movimentação");

                var promotions = _promotionService.GetActivePromotions(dto.Productid);

                decimal unitPrice = product.Price;
                decimal originalPrice = unitPrice;
                foreach (var promotion in promotions)
                {
                    unitPrice = ApplyPromotion(unitPrice, promotion);
                }

                decimal totalDiscount = originalPrice - unitPrice;

                _productService.AjustarStock(product.Id, -dto.Qty);

                var stockLogDto = new StockLogDTO
                {
                    Productid = dto.Productid,
                    Qty = -dto.Qty,
                    Createdat = DateTime.Now
                };
                _stockLogService.InsertStockLog(stockLogDto);

                var sale = _mapper.Map<TbSale>(dto);

                sale.Code = code;
                sale.Price = product.Price;
                sale.Discount = totalDiscount;
                sale.Createat = currentTime;

                _dbContext.Add(sale);
                sales.Add(sale);
            }

            _dbContext.SaveChanges();

            return sales;
        }


        public TbSale GetByCode(string code)
        {
            var existingEntity = _dbContext.TbSales.FirstOrDefault(c => c.Code == code);
            if (existingEntity == null)
            {
                throw new NotFoundException("Nenhuma venda encontrada para este código");
            }
            return existingEntity;
        }

        public List<SalesReportDTO> GetSalesReportByPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                throw new BadRequestException("As datas de início e fim são obrigatórias.");
            }

            var query = from sale in _dbContext.TbSales
                        join product in _dbContext.TbProducts on sale.Productid equals product.Id
                        where sale.Createat >= startDate && sale.Createat < endDate.AddDays(1)
                        select new SalesReportDTO
                        {
                            SaleCode = sale.Code,
                            ProductDescription = product.Description,
                            Price = sale.Price,
                            Quantity = sale.Qty,
                            SaleDate = sale.Createat
                        };

            if (!query.Any())
            {
                throw new NotFoundException("Nenhuma venda encontrada para o período.");
            }

            return query.ToList();
        }

        public decimal ApplyPromotion(decimal price, TbPromotion promotion)
        {
            decimal discountedPrice = price;

            switch (promotion.Promotiontype)
            {
                case 0:
                    discountedPrice = price * (1 - promotion.Value / 100);
                    break;
                case 1:
                    discountedPrice = price - promotion.Value;
                    break;
                default:
                    discountedPrice = price;
                    break;
            }

            return discountedPrice;
        }
    }
}
