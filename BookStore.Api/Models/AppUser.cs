using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? PasswordHash { get; set; }

        public string Role { get; set; } = "User"; // User, Admin

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}


