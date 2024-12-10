using Microsoft.AspNetCore.Http;

namespace FinalApp.ViewModels
{
    public class RegisterViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; }
        public string? MobileNumber { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfileImage { get; set; } 
    }
}
