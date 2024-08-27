using Common.DDD.Abstration.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CQRS.Abstration.command
{
    //
    // Summary:
    //     An object that is sent to the domain for a state change which is handled by a
    //     command handler.
    public interface ICommand : ICommandBase, IRequest<CommandResult>, IBaseRequest
    {
    }

    public interface ICommand<TEntity> : ICommandBase<TEntity>, ICommandBase, IRequest<CommandResult<TEntity>>, IBaseRequest where TEntity : Entity
    {
    }
}
