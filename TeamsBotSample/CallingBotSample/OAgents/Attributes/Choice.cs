using CallingBotSample.OAgents.Events;
using System;

namespace CallingBotSample.OAgents.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DispatcherChoice(string name, string description, EventType dispatchToEvent) : Attribute
    {
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public EventType DispatchToEvent { get; set; } = dispatchToEvent;
    }
}
