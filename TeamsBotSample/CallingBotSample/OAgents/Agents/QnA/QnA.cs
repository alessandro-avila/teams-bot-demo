using CallingBotSample.OAgents.Events;
using CallingBotSample.OAgents.Extensions;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.Agents.QnA;
[ImplicitStreamSubscription(Consts.OrleansNamespace)]
public class QnA([PersistentState("state", "messages")] IPersistentState<AgentState<QnAState>> state,
    ILogger<QnA> logger,
    [FromKeyedServices("QnAKernel")] Kernel kernel,
    [FromKeyedServices("QnAMemory")] ISemanticTextMemory memory) : AiAgent<QnAState>(state, memory, kernel)
{
    private readonly ILogger<QnA> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override string Namespace => Consts.OrleansNamespace;

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
            case nameof(EventType.QnARequested):
                var ssc = item.GetAgentData();
                string? userId = ssc.UserId;
                string? message = ssc.UserMessage;
                string? id = ssc.Id;

                _logger.LogInformation($"userId: {userId}, message: {message}");
                if (userId == null || message == null)
                {
                    _logger.LogWarning("[{Agent}]:[{EventType}]:[{EventData}]. Input is missing.", nameof(Dispatcher), item.Type, item.Data);
                    return;
                }

                _logger.LogInformation("[{Agent}]:[{EventType}]:[{EventData}]", nameof(QnA), nameof(EventType.QnARequested), message);
                await SendAnswerEvent(id, userId, $"Please wait...");

                var context = new KernelArguments { ["input"] = AppendChatHistory(message) };
                var instruction = "Consider the following knowledge:!vfcon106047!";
                var enhancedContext = await AddKnowledge(instruction, "vfcon106047", context);
                string answer = await CallFunction(QnAPrompts.Answer, enhancedContext);

                await SendAnswerEvent(id, userId, answer);
                break;

            default:
                break;
        }
    }

    private async Task SendAnswerEvent(string id, string userId, string message)
    {
        await PublishEvent(Consts.OrleansNamespace, id, new Event
        {
            Type = nameof(EventType.QnARetrieved),
            Data = new Dictionary<string, string> {
                { nameof(userId), userId },
                { nameof(message), message }
            }
        });
    }
}