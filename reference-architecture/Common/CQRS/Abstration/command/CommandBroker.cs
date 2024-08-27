using Common.DDD.Abstration.Entities;
using MediatR;


namespace Common.CQRS.Abstration.command
{
    public class CommandBroker : ICommandBroker
    {
        private readonly IMediator _mediator;

        //
        // Summary:
        //     Constructor.
        //
        // Parameters:
        //   mediator:
        //     Mediator for sending commands to handlers.
        public CommandBroker(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<CommandResult> SendAsync(ICommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return new CommandResult(CommandOutcome.NotHandled, new Dictionary<string, string[]> {
            {
                ex.GetType().Name,
                new string[1] { ex.Message }
            } });
            }
        }

        public async Task<CommandResult<TEntity>> SendAsync<TEntity>(ICommand<TEntity> command) where TEntity : Entity
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return new CommandResult<TEntity>(CommandOutcome.NotHandled, new Dictionary<string, string[]> {
            {
                ex.GetType().Name,
                new string[1] { ex.Message }
            } });
            }
        }
    }
}
