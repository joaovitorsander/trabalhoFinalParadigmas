using System;

namespace APITrabalhoFinal.Services.DTOs
{
    public class StockLogResultDTO
    {
        public DateTime Date { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
