using BookStore.Api.Dtos;
using FluentValidation;

namespace BookStore.Api.Validation
{
    public class BookCreateValidator : AbstractValidator<BookCreateDto>
    {
        public BookCreateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Author).MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1_000_000);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.ImageUrl).MaximumLength(300).When(x => x.ImageUrl != null);
        }
    }

    public class BookUpdateValidator : AbstractValidator<BookUpdateDto>
    {
        public BookUpdateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Author).MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1_000_000);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.ImageUrl).MaximumLength(300).When(x => x.ImageUrl != null);
        }
    }
}


