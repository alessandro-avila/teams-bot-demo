using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CallingBotSample.OAgents.Options
{
    public class CosmosDbOptions
    {
        public string? AccountUri { get; set; }

        public string? AccountKey { get; set; }

        public IEnumerable<CosmosDbContainerOptions>? Containers { get; set; }
    }
}
