using MediatR;


namespace Common.CQRS.Abstration.Queries
{
    //
    // Summary:
    //     An object that is sent to the domain to retrieve data which is handled by a query
    //     handler.
    //
    // Type parameters:
    //   TQueryResult:
    //     Query result type.
    public interface IQuery<out TQueryResult> : IRequest<TQueryResult>, IBaseRequest
    {
    }
}
