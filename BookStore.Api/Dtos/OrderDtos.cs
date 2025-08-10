using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Dtos
{
    public record OrderItemCreateDto(
        [Range(1, int.MaxValue)] int BookId,
        [Range(1, 1000)] int Quantity,
        [Range(0, 1_000_000)] decimal UnitPrice
    );

    public record OrderCreateDto(
        [Range(1, int.MaxValue)] int UserId,
        List<OrderItemCreateDto> Items
    );
}


