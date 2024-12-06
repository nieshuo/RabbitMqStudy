using FluentValidation;
using Microservice.Common.CQRS;

namespace Microservice.Inventory.Api.Commonds.UpdateWareInventory
{
    public record UpdateWareInventoryModel(
        long ProductId,
        int Quantity
    );
    public record UpdateWareInventoryCommand(UpdateWareInventoryModel model)
        : ICommand<UpdateWareInventoryResult>;

    public record UpdateWareInventoryResult(long Id);

    public class UpdateWareInventoryCommandValidator : AbstractValidator<UpdateWareInventoryCommand>
    {
        public UpdateWareInventoryCommandValidator()
        {
            RuleFor(x => x.model.ProductId).NotNull().WithMessage("ProductId is required");
            RuleFor(x => x.model.Quantity).NotNull().WithMessage("Quantity is required");
        }
    }
}
