using MediatR;

namespace Microservice.Common.CQRS
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
        where TResponse : notnull
    {
    }
}
