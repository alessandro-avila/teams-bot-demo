using CallingBotSample.OAgents.Options;
using Microsoft.SemanticKernel;
using System;

namespace CallingBotSample.OAgents.AgentsConfigurationFactory
{
    internal class DispatcherAgentConfiguration : IAgentConfiguration
    {
        public void ConfigureOpenAI(OpenAIOptions options)
        {
        }

        public void ConfigureKernel(Kernel kernel, IServiceProvider serviceProvider)
        {
        }
    }
}