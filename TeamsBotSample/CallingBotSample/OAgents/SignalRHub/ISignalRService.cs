using System.Threading.Tasks;

namespace CallingBotSample.OAgents.SignalRHub;
public interface ISignalRService
{
    Task SendMessageToClient(string messageId, string userId, string message, AgentType senderType);
}
