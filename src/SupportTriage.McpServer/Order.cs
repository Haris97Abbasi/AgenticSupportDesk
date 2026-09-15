namespace SupportTriage.McpServer;

public sealed class Order
{
    public required string OrderNumber { get; init; }
    public required string Status { get; init; }
    public required string ExpectedDeliveryDate { get; init; }
    public required string TrackingNote { get; init; }
}
