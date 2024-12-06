using FluentValidation;
using Microservice.Common.CQRS;

namespace Microservice.Bill.Api.Commonds.AddOutbill
{
    public record AddOutbillModel(
        long ProductId, 
        int Quantity,
        string? Notes
    );
    public record AddOutbillCommand(AddOutbillModel model)
        : ICommand<AddOutbillResult>;

    public record AddOutbillResult(long? Id);

    public class AddOutbillCommandValidator : AbstractValidator<AddOutbillCommand>
    {
        public AddOutbillCommandValidator()
        {
            RuleFor(x => x.model.ProductId).NotNull().WithMessage("ProductId is required");
            RuleFor(x => x.model.Quantity).NotNull().WithMessage("Quantity is required");
        }
    }
}
