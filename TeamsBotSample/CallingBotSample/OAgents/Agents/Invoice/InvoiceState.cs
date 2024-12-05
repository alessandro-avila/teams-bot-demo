using Orleans;

namespace CallingBotSample.OAgents.Agents.Invoice;

public class InvoiceState
{
    [Id(0)]
    public string? InvoiceId { get; set; }
}

