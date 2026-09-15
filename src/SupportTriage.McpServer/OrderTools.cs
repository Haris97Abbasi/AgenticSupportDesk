using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SupportTriage.McpServer;

[McpServerToolType]
public static class OrderTools
{
    private static readonly Dictionary<string, Order> Orders = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ORD-1024"] = new Order
        {
            OrderNumber = "ORD-1024",
            Status = "In Transit",
            ExpectedDeliveryDate = "2026-09-20",
            TrackingNote = "Package left the regional distribution center on 2026-09-14 but has not scanned in at the next facility for 24+ hours."
        },
        ["ORD-2048"] = new Order
        {
            OrderNumber = "ORD-2048",
            Status = "Delivered",
            ExpectedDeliveryDate = "2026-09-10",
            TrackingNote = "Delivered to front porch, signed by resident."
        },
        ["ORD-3072"] = new Order
        {
            OrderNumber = "ORD-3072",
            Status = "Processing",
            ExpectedDeliveryDate = "2026-09-22",
            TrackingNote = "Order confirmed, awaiting warehouse fulfillment."
        }
    };

    [McpServerTool, Description("Looks up an order by its order number and returns its status, expected delivery date, and latest tracking note. Returns a not-found message if the order number is unknown.")]
    public static string GetOrder(
        [Description("The order number to look up, e.g. ORD-1024.")] string orderNumber)
    {
        if (Orders.TryGetValue(orderNumber, out var order))
        {
            return $"Order {order.OrderNumber}: status={order.Status}, expectedDeliveryDate={order.ExpectedDeliveryDate}, trackingNote=\"{order.TrackingNote}\"";
        }

        return $"No order found with number '{orderNumber}'. Do not guess or invent order details.";
    }
}
