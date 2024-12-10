namespace FinalApp.ViewModels
{
    public class SelectedItemsViewModel
    {
        public Guid UserId { get; set; }
        public IEnumerable<FinalApp.Models.Item>? Items { get; set; }
    }
}
