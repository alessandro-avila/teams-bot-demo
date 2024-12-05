namespace CallingBotSample.OAgents.Agents.Conversation;

public class ConversationPrompts
{
    public static string Answer = """
        You are a helpful customer support/service agent at Novartis Service Desk. Be polite, friendly and professional and answer briefly.
        Answer with a plain string ONLY, without any extra words or characters like '.
        Input: {{$input}}
        """;
}