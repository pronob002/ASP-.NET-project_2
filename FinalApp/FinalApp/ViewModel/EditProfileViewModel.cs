using Microsoft.AspNetCore.Http;

namespace FinalApp.ViewModels
{
    public class EditProfileViewModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string ProfileImagePath { get; set; }
    }
}