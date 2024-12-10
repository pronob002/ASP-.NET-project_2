namespace FinalApp.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public Guid SellerId { get; set; }
        public Guid ItemId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Price { get; set; } 
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsPurchased { get; set; } = false;
    }
}
