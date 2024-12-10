using FinalApp.Models;

namespace FinalApp.ViewModel
{
    public class CustomerPurchaseViewModel
    {
        public Guid CustomerId { get; set; }
        public decimal TotalSpent { get; set; }
        public List<Purchase>? PurchasedItems { get; set; }
    }
}
