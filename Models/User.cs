using System.ComponentModel.DataAnnotations;

namespace ShopX.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; } // For demo only — use hashing in real apps
        public string Email { get; set; }
    }
}
