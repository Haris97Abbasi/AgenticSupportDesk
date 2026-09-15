using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using SupportTriage.Web.Models;

namespace SupportTriage.Web.Services;

public sealed class SupportAgentService
{
    private const string AgentInstructions =
        "You are a support assistant. Never invent customer or order information. " +
        "Use available tools when factual account/order information is required.";

    private readonly AIAgent _agent;
    private AgentSession? _session;

    public SupportAgentService(IConfiguration configuration, McpToolProvider mcpToolProvider)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI:ApiKey is not configured. Run: dotnet user-secrets set \"OpenAI:ApiKey\" \"sk-...\" in SupportTriage.Web.");
        }

        var openAiClient = new OpenAIClient(apiKey);
        IChatClient chatClient = openAiClient.GetChatClient(model).AsIChatClient();

        List<AITool> tools = [CustomerLookupService.Tool, .. mcpToolProvider.Tools];

        _agent = new ChatClientAgent(
            chatClient,
            instructions: AgentInstructions,
            name: "SupportTriageAgent",
            description: "Support triage assistant that looks up customer and order data to help resolve support cases.",
            tools: tools);
    }

    private async Task<AgentSession> GetSessionAsync(CancellationToken cancellationToken)
    {
        _session ??= await _agent.CreateSessionAsync(cancellationToken);
        return _session;
    }

    public async Task<string> SendMessageAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(cancellationToken);
        AgentResponse response = await _agent.RunAsync(userMessage, session, cancellationToken: cancellationToken);
        return response.Text;
    }

    public async Task<SupportCaseResult> AnalyzeCaseAsync(string analysisPrompt, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(cancellationToken);
        AgentResponse<SupportCaseResult> result =
            await _agent.RunAsync<SupportCaseResult>(analysisPrompt, session, cancellationToken: cancellationToken);
        return result.Result;
    }
}
