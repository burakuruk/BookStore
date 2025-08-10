namespace BookStore.Api.Dtos
{
    public record BookCreateDto(
        string Title,
        string? Author,
        decimal Price,
        string? ImageUrl,
        int CategoryId
    );

    public record BookUpdateDto(
        string Title,
        string? Author,
        decimal Price,
        string? ImageUrl,
        int CategoryId
    );
}


