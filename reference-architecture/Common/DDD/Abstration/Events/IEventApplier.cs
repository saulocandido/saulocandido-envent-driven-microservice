using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DDD.Abstration.Events
{

    //
    // Summary:
    //     Applies an event by muting entity state.
    //
    // Type parameters:
    //   TEvent:
    //     Event type.
    public interface IEventApplier<in TEvent> where TEvent : class, IDomainEvent
    {
        //
        // Summary:
        //     Applies a change to the given entity from the specified domain event.
        //
        // Parameters:
        //   domainEvent:
        //     Domain event to apply.
        void Apply(TEvent domainEvent);
    }
}
