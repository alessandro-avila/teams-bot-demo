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
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.Agents.Invoice;

[ImplicitStreamSubscription(Consts.OrleansNamespace)]
public class Invoice : AiAgent<InvoiceState>
{
    private readonly ILogger<Invoice> _logger;

    protected override string Namespace => Consts.OrleansNamespace;

    public Invoice([PersistentState("state", "messages")] IPersistentState<AgentState<InvoiceState>> state,
        ILogger<Invoice> logger,
        [FromKeyedServices("InvoiceKernel")] Kernel kernel,
        [FromKeyedServices("InvoiceMemory")] ISemanticTextMemory memory)
    : base(state, memory, kernel)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async override Task HandleEvent(Event item)
    {
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

        switch (item.Type)
        {
            case nameof(EventType.InvoiceRequested):
                {
                    await SendAnswerEvent(id, userId, $"Please wait while I look up the details for invoice...");
                    _logger.LogInformation("[{Agent}]:[{EventType}]:[{EventData}]", nameof(Invoice), nameof(EventType.InvoiceRequested), message);

                    var querycontext = new KernelArguments { ["input"] = AppendChatHistory(message) };
                    var instruction = "Consider the following knowledge:!invoices!";
                    var enhancedContext = await AddKnowledge(instruction, "invoices", querycontext);
                    string answer = await CallFunction(InvoicePrompts.InvoiceRequest, enhancedContext);
                    await SendAnswerEvent(id, userId, answer);
                    break;
                }
            default:
                break;
        }
    }

    private async Task SendAnswerEvent(string id, string userId, string message)
    {
        await PublishEvent(Namespace, id, new Event
        {
            Type = nameof(EventType.InvoiceRetrieved),
            Data = new Dictionary<string, string>
            {
                { nameof(userId), userId },
                { nameof(message),  message }
            }
        });
    }
}