﻿using CallingBotSample.OAgents.Events;
using CallingBotSample.OAgents.Options;
using Microsoft.AI.Agents.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Interactions : ControllerBase
    {
        private readonly IClusterClient _client;

        public Interactions(IClusterClient client)
        {
            _client = client;
        }

        // POST api/<Post>/5
        [HttpPost("{userId}")]
        public async Task<string> Post(string userId, [FromBody] string userMessage)
        {
            var streamProvider = _client.GetStreamProvider("StreamProvider");
            var streamId = StreamId.Create(Consts.OrleansNamespace, userId); // to change
            var stream = streamProvider.GetStream<Event>(streamId);

            var data = new Dictionary<string, string>
            {
                { nameof(userId), userId.ToString() },
                { nameof(userMessage), userMessage },
            };

            await stream.OnNextAsync(new Event
            {
                Type = nameof(EventType.UserChatInput),
                Data = data
            });

            return $"Task {userId} accepted";
        }
    }
}
