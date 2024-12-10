namespace FinalApp.Models
{
    public class Item
    {
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? SellerId { get; set; }
        public string? ItemImagePath { get; set; }
        public bool IsSold { get; set; } = false;
    }
}
