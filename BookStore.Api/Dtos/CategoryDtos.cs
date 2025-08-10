namespace BookStore.Api.Dtos
{
    public record CategoryCreateDto(
        string Name,
        string? Description
    );

    public record CategoryUpdateDto(
        string Name,
        string? Description
    );
}


