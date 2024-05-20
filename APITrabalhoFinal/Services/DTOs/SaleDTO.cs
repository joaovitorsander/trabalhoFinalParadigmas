using System;

namespace APITrabalhoFinal.Services.DTOs
{
    public class SaleDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public DateTime Createat { get; set; }

        public int Productid { get; set; }

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public decimal Discount { get; set; }
    }
}
