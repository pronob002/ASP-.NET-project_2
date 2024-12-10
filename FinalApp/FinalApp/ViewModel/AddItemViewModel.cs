using Microsoft.AspNetCore.Http;

namespace FinalApp.ViewModels
{
    public class AddItemViewModel
    {
        public string? ItemName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? ItemImage { get; set; }
    }
}
