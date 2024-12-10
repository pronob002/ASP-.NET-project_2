namespace FinalApp.ViewModels
{
    public class CustomerDetailViewModel
    {
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
        public List<ItemDetailViewModel>? ItemsPurchased { get; set; }
    }

    public class ItemDetailViewModel
    {
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal Price { get; set; }
    }
}
