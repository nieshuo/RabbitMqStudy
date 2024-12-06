using FluentValidation;
using Microservice.Common.CQRS;

namespace Microservice.Bill.Api.Commonds.AddStock
{
    public record AddStockModel(
        long ProductId, 
        int Quantity,
        string? Notes
    );
    public record AddStockCommand(AddStockModel model)
        : ICommand<AddStockResult>;

    public record AddStockResult(long? Id);

    public class AddStockCommandValidator : AbstractValidator<AddStockCommand>
    {
        public AddStockCommandValidator()
        {
            RuleFor(x => x.model.ProductId).NotNull().WithMessage("ProductId is required");
            RuleFor(x => x.model.Quantity).NotNull().WithMessage("Quantity is required");
        }
    }
}
