using CallingBotSample.OAgents.Options;
using Microsoft.SemanticKernel;
using System;

namespace CallingBotSample.OAgents.AgentsConfigurationFactory
{
    public interface IAgentConfiguration
    {
        void ConfigureOpenAI(OpenAIOptions options);
        void ConfigureKernel(Kernel kernel, IServiceProvider serviceProvider);
    }
}
