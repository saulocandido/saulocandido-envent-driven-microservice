using MediatR;


namespace Common.CQRS.Abstration.Queries
{
    //
    // Summary:
    //     Query handler.
    //
    // Type parameters:
    //   TQuery:
    //     Query type.
    //
    //   TQueryResult:
    public interface IQueryHandler<in TQuery, TQueryResult> : IRequestHandler<TQuery, TQueryResult> where TQuery : class, IQuery<TQueryResult>
    {
    }
}
