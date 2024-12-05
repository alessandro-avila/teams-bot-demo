using CallingBotSample.OAgents.Events;
using CallingBotSample.OAgents.Options;
using Microsoft.AI.Agents.Abstractions;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallingBotSample.Services.AgenticServices
{
    public class DispatcherService(IClusterClient clusterClient, ILogger<DispatcherService> logger)
    {
        public async Task SendMessageAsync(string userId, string userMessage)
        {
            var streamProvider = clusterClient.GetStreamProvider("StreamProvider");
            var streamId = StreamId.Create(Consts.OrleansNamespace, userId);
            var stream = streamProvider.GetStream<Event>(streamId);

            //var responseTask = new TaskCompletionSource<string>();

            //// Subscribe to the stream to receive the agent's response
            //var subscriptionHandle = await stream.SubscribeAsync(
            //    async (evt, token) =>
            //    {
            //        if (evt.Type == nameof(EventType.AgentResponse))
            //        {
            //            if (evt.Data.TryGetValue("message", out var responseMessage))
            //            {
            //                responseTask.SetResult(responseMessage);
            //            }
            //        }

            //    });

            // Send the user's message to the dispatcher agent
            var data = new Dictionary<string, string>
            {
                { nameof(userId), userId },
                { nameof(userMessage), userMessage },
            };

            await stream.OnNextAsync(new Event
            {
                Type = nameof(EventType.UserChatInput),
                Data = data
            });

            // Wait for the agent's response
            

            //// Unsubscribe from the stream
            //await subscriptionHandle.UnsubscribeAsync();

        }
    }
}
