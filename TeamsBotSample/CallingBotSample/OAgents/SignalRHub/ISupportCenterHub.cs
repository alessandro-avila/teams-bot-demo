using Orleans;
using System.Threading.Tasks;

namespace CallingBotSample.OAgents.SignalRHub;

public interface ISupportCenterHub
{
    public Task ConnectToAgent(string userId);

    public Task ChatMessage(ChatMessage frontEndMessage, IClusterClient clusterClient);

    public Task SendMessageToSpecificClient(string userId, string message);
}
