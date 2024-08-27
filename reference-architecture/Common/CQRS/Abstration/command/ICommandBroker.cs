using Common.DDD.Abstration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CQRS.Abstration.command
{
    //
    // Summary:
    //     Send commands to be handled by a command handler.
    public interface ICommandBroker
    {
        //
        // Summary:
        //     Send a command to be handled by a command handler.
        //
        // Parameters:
        //   command:
        //     The command.
        //
        // Returns:
        //     The command result.
        Task<CommandResult> SendAsync(ICommand command);

        //
        // Summary:
        //     Send a command to be handled by a command handler.
        //
        // Parameters:
        //   command:
        //     The command.
        //
        // Type parameters:
        //   TEntity:
        //     Entity type.
        //
        // Returns:
        //     The command result.
        Task<CommandResult<TEntity>> SendAsync<TEntity>(ICommand<TEntity> command) where TEntity : Entity;
    }
}
