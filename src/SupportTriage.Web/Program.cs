using SupportTriage.Web.Components;
using SupportTriage.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Launch the local Order MCP server (stdio transport) and connect to it once at startup.
var mcpServerProjectPath = builder.Configuration["Mcp:ServerProjectPath"]
    ?? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "SupportTriage.McpServer", "SupportTriage.McpServer.csproj"));
var mcpToolProvider = await McpToolProvider.CreateAsync(mcpServerProjectPath);
builder.Services.AddSingleton(mcpToolProvider);

var app = builder.Build();

app.Logger.LogInformation(
    "Connected to Order MCP server. Discovered tools: {Tools}",
    string.Join(", ", mcpToolProvider.Tools.Select(t => t.Name)));

app.Lifetime.ApplicationStopping.Register(() =>
    mcpToolProvider.DisposeAsync().AsTask().GetAwaiter().GetResult());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
