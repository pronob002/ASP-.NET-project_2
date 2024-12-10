using Microsoft.AspNetCore.Http;

public class EditItemViewModel
{
    public Guid ItemId { get; set; } 
    public string ItemName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile ItemImage { get; set; }
    public string ExistingItemImagePath { get; set; } 
}