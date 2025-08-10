namespace BookStore.Api.Dtos
{
    public record LoginRequest(string Username, string Password);
    public record TokenResponse(string Token, string Role, string Username, DateTime ExpiresAtUtc);
}


