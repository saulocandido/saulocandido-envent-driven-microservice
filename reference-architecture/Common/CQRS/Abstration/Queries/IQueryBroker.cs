

namespace Common.CQRS.Abstration.Queries
{
    //
    // Summary:
    //     Send queries to be handled by a query handler.
    public interface IQueryBroker
    {
        //
        // Summary:
        //     Send a query to be handled by a query handler.
        //
        // Parameters:
        //   query:
        //     The query.
        //
        // Type parameters:
        //   TQueryResult:
        //     Query result type.
        //
        // Returns:
        //     The query result.
        Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> query);
    }
}
