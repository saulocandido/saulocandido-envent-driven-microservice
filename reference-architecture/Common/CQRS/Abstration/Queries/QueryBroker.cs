using MediatR;


namespace Common.CQRS.Abstration.Queries
{
    public class QueryBroker : IQueryBroker
    {
        private readonly IMediator _mediator;

        //
        // Summary:
        //     Constructor.
        //
        // Parameters:
        //   mediator:
        //     Mediator for sending queries to handlers.
        public QueryBroker(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> query)
        {
            return await _mediator.Send(query);
        }
    }
}
