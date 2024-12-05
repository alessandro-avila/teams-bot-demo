using CallingBotSample.OAgents.Options;
using Microsoft.SemanticKernel;
using System;

namespace CallingBotSample.OAgents.AgentsConfigurationFactory
{
    public class InvoiceAgentConfiguration : IAgentConfiguration
    {
        public void ConfigureOpenAI(OpenAIOptions options)
        {
            options.ChatDeploymentOrModelId = options.InvoiceDeploymentOrModelId ?? options.ChatDeploymentOrModelId;
        }

        public void ConfigureKernel(Kernel kernel, IServiceProvider serviceProvider)
        {
        }
    }
}
