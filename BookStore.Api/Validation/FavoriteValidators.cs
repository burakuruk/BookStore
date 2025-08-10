using BookStore.Api.Dtos;
using FluentValidation;

namespace BookStore.Api.Validation
{
    public class FavoriteCreateValidator : AbstractValidator<FavoriteCreateDto>
    {
        public FavoriteCreateValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.BookId).GreaterThan(0);
        }
    }
}


