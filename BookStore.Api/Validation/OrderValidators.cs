using BookStore.Api.Dtos;
using FluentValidation;

namespace BookStore.Api.Validation
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.Items).NotNull().NotEmpty();
            RuleForEach(x => x.Items).SetValidator(new OrderItemCreateValidator());
        }
    }

    public class OrderItemCreateValidator : AbstractValidator<OrderItemCreateDto>
    {
        public OrderItemCreateValidator()
        {
            RuleFor(x => x.BookId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(1000);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        }
    }
}


