using APITrabalhoFinal.DataBase.Models;
using System.Collections.Generic;

namespace APITrabalhoFinal.Services.DTOs
{
    public class ProductDTO
    { 
        public string Description { get; set; }

        public string Barcode { get; set; }

        public string Barcodetype { get; set; }

        public int Stock { get; set; }

        public decimal Price { get; set; }

        public decimal Costprice { get; set; }
    }
}
