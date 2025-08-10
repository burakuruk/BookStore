namespace BookStore.Api.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser? User { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


