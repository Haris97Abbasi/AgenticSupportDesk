using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace SupportTriage.Web.Services;

public sealed class McpToolProvider : IAsyncDisposable
{
    private readonly McpClient _client;

    public IReadOnlyList<AIFunction> Tools { get; }

    private McpToolProvider(McpClient client, IReadOnlyList<AIFunction> tools)
    {
        _client = client;
        Tools = tools;
    }

    public static async Task<McpToolProvider> CreateAsync(string mcpServerProjectPath, CancellationToken cancellationToken = default)
    {
        var transport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "SupportTriage.McpServer",
            Command = "dotnet",
            Arguments = ["run", "--no-launch-profile", "--project", mcpServerProjectPath]
        });

        var client = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
        var tools = await client.ListToolsAsync(cancellationToken: cancellationToken);

        return new McpToolProvider(client, [.. tools]);
    }

    public ValueTask DisposeAsync() => _client.DisposeAsync();
}
