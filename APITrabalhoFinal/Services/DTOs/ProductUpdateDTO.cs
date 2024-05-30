namespace APITrabalhoFinal.Services.DTOs
{
    public class ProductUpdateDTO
    {
        public string Description { get; set; }
        public string Barcode { get; set; }
        public string Barcodetype { get; set; }
        public decimal Price { get; set; }
        public decimal Costprice { get; set; }
    }
}
