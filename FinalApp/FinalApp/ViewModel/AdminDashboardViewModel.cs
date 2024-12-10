using System.Collections.Generic;
using FinalApp.Models;

namespace FinalApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<User>? Customers { get; set; }
        public IEnumerable<User>? Sellers { get; set; }
        public IEnumerable<Item>? Items { get; set; }
        public Dictionary<Guid, ItemPurchaseDetail>? ItemPurchaseDetails { get; set; }
    }

    public class ItemPurchaseDetail
    {
        public string? ItemName { get; set; }
        public int PurchaseCount { get; set; }
    }
}
