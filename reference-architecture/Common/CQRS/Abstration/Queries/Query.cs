using MediatR;

namespace Common.CQRS.Abstration.Queries
{
    public abstract record Query<TQueryResult> : IQuery<TQueryResult>, IRequest<TQueryResult>, IBaseRequest;
}
