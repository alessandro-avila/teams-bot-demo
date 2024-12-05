using CallingBotSample.OAgents.Events;
using CallingBotSample.OAgents.Extensions;
using CallingBotSample.OAgents.Options;
using CallingBotSample.OAgents.SignalRHub;
using Microsoft.AI.Agents.Abstractions;
using Microsoft.AI.Agents.Orleans;
using Microsoft.Extensions.Logging;
using Orleans;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.Agents.Response;

[ImplicitStreamSubscription(Consts.OrleansNamespace)]
public class Response(ILogger<Response> logger) : Agent
{
    protected override string Namespace => Consts.OrleansNamespace;
    private readonly ConcurrentDictionary<string, AgentType> _eventTypeToSenderTypeMapping = new()
    {
        [nameof(EventType.QnARetrieved)] = AgentType.QnA,
        [nameof(EventType.QnANotification)] = AgentType.QnA,
        [nameof(EventType.InvoiceRetrieved)] = AgentType.Invoice,
        [nameof(EventType.InvoiceNotification)] = AgentType.Invoice,
        [nameof(EventType.DispatcherNotification)] = AgentType.Dispatcher,
        [nameof(EventType.CustomerInfoRetrieved)] = AgentType.CustomerInfo,
        [nameof(EventType.CustomerInfoNotification)] = AgentType.CustomerInfo,
        [nameof(EventType.DiscountRetrieved)] = AgentType.Discount,
        [nameof(EventType.DiscountNotification)] = AgentType.Discount,
        [nameof(EventType.AgentNotification)] = AgentType.Notification,
        [nameof(EventType.ConversationRetrieved)] = AgentType.Conversation,
        [nameof(EventType.Unknown)] = AgentType.Unknown
    };

    public override async Task HandleEvent(Event item)
    {
        string? userId = item.Data.GetValueOrDefault<string>("userId");
        string? message = item.Data.GetValueOrDefault<string>("message");

        if (userId == null)
        {
            return;
        }

        if (!_eventTypeToSenderTypeMapping.TryGetValue(item.Type, out AgentType agentType))
        {
            return;
        }

        if (agentType == AgentType.Unknown)
        {
            logger.LogWarning("[{Agent}]:[{EventType}]:[{EventData}]. This event is not supported.", nameof(Response), item.Type, item.Data);
            message = "Sorry, I don't know how to handle this request. Try to rephrase it.";
        }

        await PublishEvent(Namespace, userId, new Event
        {
            Type = EventType.AgentResponse.ToString(),
            Data = new Dictionary<string, string>
            {
                { nameof(userId), userId },
                { nameof(message),  message },
                { nameof(agentType), agentType.ToString() }
            }
        });
    }
}