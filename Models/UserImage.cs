using System;

namespace ShopX.Models
{
    public class UserImage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    }
}
