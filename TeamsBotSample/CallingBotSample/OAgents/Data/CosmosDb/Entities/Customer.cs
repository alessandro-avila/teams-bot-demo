﻿using CallingBotSample.OAgents.Data.Entities;
using System.Text.Json.Serialization;

namespace CallingBotSample.OAgents.Data.CosmosDb.Entities
{
    public class Customer : Entity
    {
        [JsonPropertyName(nameof(Name))]
        public string? Name { get; set; }

        [JsonPropertyName(nameof(Email))]
        public string? Email { get; set; }

        [JsonPropertyName(nameof(Phone))]
        public string? Phone { get; set; }

        [JsonPropertyName(nameof(Address))]
        public string? Address { get; set; }

        public override string GetPartitionKeyValue() => Id;
    }
}
