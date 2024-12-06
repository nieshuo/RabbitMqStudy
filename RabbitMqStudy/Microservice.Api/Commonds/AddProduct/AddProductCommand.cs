using FluentValidation;
using Microservice.Common.CQRS;

namespace Microservice.Inventory.Api.Commonds.AddProduct
{
    public record AddProductModel(
        string Name,
        string? Description
    );
    public record AddProductCommand(AddProductModel model)
        : ICommand<AddProductResult>;

    public record AddProductResult(long Id);

    public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
    {
        public AddProductCommandValidator()
        {
            RuleFor(x => x.model.Name).NotEmpty().WithMessage("Name is required");
        }
    }
}
