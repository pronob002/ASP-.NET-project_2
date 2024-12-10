using System;
using System.ComponentModel.DataAnnotations;

namespace FinalApp.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }  

        public Guid UserId { get; set; }  
    }
}
