using ToolServer.Tools;

var builder = WebApplication.CreateBuilder(args);

// AddMcpServer registers the MCP server infrastructure.
// WithHttpTransport sets up two HTTP endpoints:
//   GET  /mcp  → SSE stream (client subscribes to receive server messages)
//   POST /mcp  → client sends requests (tool calls, list requests, etc.)
// WithTools<T> scans T for [McpServerTool] methods and registers them.
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<WeatherTools>()
    .WithTools<MathTools>()
    .WithTools<TimeTools>();

var app = builder.Build();

app.MapMcp("/mcp");

await app.RunAsync();
