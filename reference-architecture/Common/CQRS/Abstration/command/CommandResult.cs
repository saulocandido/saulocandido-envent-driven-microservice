using Common.CQRS.Abstration.command;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Common.CQRS.Abstration
{

    public record CommandResult<TEntity> : CommandResult where TEntity : IEntity
    {
        public List<TEntity> Entities { get; }

        public TEntity? Entity => Entities.FirstOrDefault();

        public CommandResult(CommandOutcome outcome, params TEntity[] entities)
            : base(outcome) // Call the base constructor
        {
            Entities = new List<TEntity>(entities);
            foreach (TEntity item in entities)
            {
                Entities.Add(item);
            }
        }

        public CommandResult(CommandOutcome outcome, IDictionary<string, string[]> errors, params TEntity[] entities)
            : base(outcome, errors) // Call the base constructor
        {
            Entities = new List<TEntity>(entities);
            foreach (TEntity item in entities)
            {
                Entities.Add(item);
            }
        }

        // This method is already here for the generic CommandResult
        public IActionResult ToActionResult()
        {
            return Outcome switch
            {
                CommandOutcome.Accepted => new OkObjectResult(Entity),
                CommandOutcome.InvalidCommand => new BadRequestObjectResult("Invalid command"),
                CommandOutcome.NotFound => new NotFoundResult(),
                CommandOutcome.Conflict => new ConflictObjectResult("A concurrency conflict occurred"),
                CommandOutcome.NotHandled => new StatusCodeResult(500),
                _ => new StatusCodeResult(500)
            };
        }

        public IActionResult ToActionResult(Object entity)
        {
            return Outcome switch
            {
                CommandOutcome.Accepted => new OkObjectResult(entity),
                CommandOutcome.InvalidCommand => new BadRequestObjectResult("Invalid command"),
                CommandOutcome.NotFound => new NotFoundResult(),
                CommandOutcome.Conflict => new ConflictObjectResult("A concurrency conflict occurred"),
                CommandOutcome.NotHandled => new StatusCodeResult(500),
                _ => new StatusCodeResult(500)
            };
        }
    }

    // Adding ToActionResult method to the non-generic CommandResult class
    public record CommandResult(CommandOutcome Outcome, IDictionary<string, string[]>? Errors = null)
    {
        public IActionResult ToActionResult()
        {
            return Outcome switch
            {
                CommandOutcome.Accepted => new OkResult(),
                CommandOutcome.InvalidCommand => new BadRequestObjectResult("Invalid command"),
                CommandOutcome.NotFound => new NotFoundResult(),
                CommandOutcome.Conflict => new ConflictObjectResult("A concurrency conflict occurred"),
                CommandOutcome.NotHandled => new StatusCodeResult(500),
                _ => new StatusCodeResult(500)
            };
        }
    }
}
