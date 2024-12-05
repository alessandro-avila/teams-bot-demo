using CallingBotSample.OAgents.Events;
using CallingBotSample.OAgents.Options;
using Microsoft.AI.Agents.Abstractions;
using Microsoft.AI.Agents.Orleans;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Orleans;
using Orleans.Runtime;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.Agents.Discount;

[ImplicitStreamSubscription(Consts.OrleansNamespace)]
public class Discount : AiAgent<DiscountState>
{
    private readonly ILogger<Discount> _logger;

    protected override string Namespace => Consts.OrleansNamespace;

    public Discount([PersistentState("state", "messages")] IPersistentState<AgentState<DiscountState>> state,
        ILogger<Discount> logger,
        [FromKeyedServices("DiscountKernel")] Kernel kernel,
        [FromKeyedServices("DiscountMemory")] ISemanticTextMemory memory)
    : base(state, memory, kernel)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task HandleEvent(Event item)
    {
        switch (item.Type)
        {
            case nameof(EventType.UserConnected):
                // The user reconnected, let's send the last message if we have one
                string? lastMessage = _state.State.History.LastOrDefault()?.Message;
                if (lastMessage == null)
                {
                    return;
                }
                break;
            default:
                break;
        }
    }
}