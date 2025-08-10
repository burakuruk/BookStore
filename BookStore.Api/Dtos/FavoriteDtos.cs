namespace BookStore.Api.Dtos
{
    public record FavoriteCreateDto(
        int UserId,
        int BookId
    );
}


