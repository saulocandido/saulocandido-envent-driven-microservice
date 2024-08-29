using Common.DDD.Abstration.Entities;
using MediatR;
using System.Runtime.CompilerServices;

namespace Common.CQRS.Abstration.command
{

    public abstract record Command(Guid EntityId = default(Guid), string? EntityETag = null) : ICommand, ICommandBase, IRequest<CommandResult>, IBaseRequest;


    //
    // Summary:
    //     An object that is sent to the domain for a state change which is handled by a
    //     command handler.
    //
    // Parameters:
    //   Entity:
    //     The entity that this command is about.
    //
    //   EntityId:
    //     The id of the entity that this command is about.
    //
    //   EntityETag:
    //     A unique id that must change atomically with each store of the entity.
    //
    // Type parameters:
    //   TEntity:
    //     Entity type.
    public abstract record Command<TEntity> : ICommand<TEntity>, ICommandBase<TEntity>, ICommandBase, IRequest<CommandResult<TEntity>>, IBaseRequest where TEntity : Entity
    {
        public Guid EntityId { get; }

        public string? EntityETag { get; }

        public TEntity? Entity { get; }

        //
        // Summary:
        //     An object that is sent to the domain for a state change which is handled by a
        //     command handler.
        //
        // Parameters:
        //   Entity:
        //     The entity that this command is about.
        //
        //   EntityId:
        //     The id of the entity that this command is about.
        //
        //   EntityETag:
        //     A unique id that must change atomically with each store of the entity.
        //
        // Type parameters:
        //   TEntity:
        //     Entity type.
        protected Command(TEntity? Entity = null, Guid EntityId = default(Guid), string? EntityETag = null)
        {
            this.EntityId = Entity?.Id ?? EntityId;
            this.EntityETag = Entity?.ETag ?? EntityETag;
            this.Entity = Entity;
        }

        
        public void Deconstruct(out TEntity? Entity, out Guid EntityId, out string? EntityETag)
        {
            Entity = this.Entity;
            EntityId = this.EntityId;
            EntityETag = this.EntityETag;
        }
    }
}
