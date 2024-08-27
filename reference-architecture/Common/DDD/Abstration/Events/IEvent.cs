using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DDD.Abstration.Events
{
    //
    // Summary:
    //     A statement of fact about what change has been made to the domain state.
    public interface IEvent
    {
        //
        // Summary:
        //     Unique ID of the event.
        Guid Id { get; set; }
    }
}
