using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.eventDriven.Abistraction.MessageBus
{
    public class EventBusOptions
    {
        public string PubSubName { get; set; } = "pubsub";
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public int MaxConcurrentHandlers { get; set; } = Environment.ProcessorCount;
    }
}
