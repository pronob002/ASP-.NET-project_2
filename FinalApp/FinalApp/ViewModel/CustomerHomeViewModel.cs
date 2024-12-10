namespace FinalApp.ViewModels
{
    public class CustomerHomeViewModel
    {
        public Guid UserId { get; set; }
        public IEnumerable<FinalApp.Models.Item>? Items { get; set; }
        public string? SearchQuery { get; set; }
    }
}
