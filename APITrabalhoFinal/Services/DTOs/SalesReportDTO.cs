using System;

namespace APITrabalhoFinal.Services.DTOs
{
    public class SalesReportDTO
    {
        public string SaleCode { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
