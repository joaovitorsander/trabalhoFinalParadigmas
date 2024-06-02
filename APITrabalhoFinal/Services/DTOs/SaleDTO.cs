using System;

namespace APITrabalhoFinal.Services.DTOs
{
    public class SaleDTO
    { 
        public string Code { get; set; }

        public int Productid { get; set; }

        public int Qty { get; set; }

        public decimal Discount { get; set; }
    }
}
