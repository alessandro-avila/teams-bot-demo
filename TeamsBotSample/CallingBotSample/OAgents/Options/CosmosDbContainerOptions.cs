﻿using System.ComponentModel.DataAnnotations;

namespace CallingBotSample.OAgents.Options
{
    public class CosmosDbContainerOptions
    {
        [Required]
        public string? DatabaseName { get; set; }

        [Required]
        public string? ContainerName { get; set; }

        [Required]
        public string? PartitionKey { get; set; }

        [Required]
        public string? EntityName { get; set; }
    }
}