using CallingBotSample.OAgents.Options;
using Microsoft.AI.Agents.Abstractions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CallingBotSample.Services.AgenticServices
{
    public class OrleansSubscriberService(
    IClusterClient clusterClient,
    CloudAdapter cloudAdapter,
    string botAppId,
    ILogger<OrleansSubscriberService> logger)
    {
        private readonly ConcurrentDictionary<string, ConversationReference> conversationReferences = new();
        private readonly ConcurrentDictionary<string, StreamSubscriptionHandle<Event>> subscriptions = new();
        private readonly IClusterClient clusterClient = clusterClient;
        private readonly CloudAdapter cloudAdapter = cloudAdapter;
        private readonly string botAppId = botAppId;
        private readonly ILogger<OrleansSubscriberService> logger = logger;

        public void AddConversationReference(string userId, ConversationReference conversationReference)
        {
            conversationReferences.TryAdd(userId, conversationReference);
            EnsureSubscription(userId);
        }

        private void EnsureSubscription(string userId)
        {
            if (!subscriptions.ContainsKey(userId))
            {
                var streamProvider = clusterClient.GetStreamProvider("StreamProvider");
                var streamId = StreamId.Create(Consts.OrleansNamespace, userId);
                var stream = streamProvider.GetStream<Event>(streamId);

                var subscriptionTask = stream.SubscribeAsync(async (evt, token) =>
                {
                    await HandleAgentEvent(userId, evt);
                });

                subscriptions[userId] = subscriptionTask.Result;
            }
        }

        private async Task HandleAgentEvent(string userId, Event agentEvent)
        {
            if (conversationReferences.TryGetValue(userId, out var conversationReference))
            {
                await cloudAdapter.ContinueConversationAsync(
                    botAppId,
                    conversationReference,
                    async (turnContext, cancellationToken) =>
                    {
                        // Extract the message and agent type
                        if (agentEvent.Data.TryGetValue("message", out var message) &&
                            agentEvent.Data.TryGetValue("agentType", out var agentType))
                        {
                            // Create a message with the agent type
                            var replyText = $"*{agentType} Agent*: {message}";
                            await turnContext.SendActivityAsync(MessageFactory.Text(replyText), cancellationToken);
                        }
                    },
                    default);
            }
            else
            {
                logger.LogWarning("ConversationReference not found for userId: {UserId}", userId);
            }
        }
    }
}
