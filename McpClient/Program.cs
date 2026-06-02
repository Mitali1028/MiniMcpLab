using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

// Aspire injects the tool server's URL via this environment variable when using WithReference().
// Fallback to a command-line arg or hardcoded default when running manually.
var toolServerBase = Environment.GetEnvironmentVariable("services__tool-server__http__0")
    ?? args.FirstOrDefault()
    ?? "http://localhost:5100";

var mcpEndpoint = $"{toolServerBase}/mcp";

Console.WriteLine($"Connecting to MCP server at: {mcpEndpoint}");
Console.WriteLine();

// HttpClientTransport establishes two HTTP connections:
//   GET {endpoint}  → opens an SSE stream to receive server-to-client messages
//   POST {endpoint} → sends client-to-server requests (JSON-RPC over HTTP)
var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri(mcpEndpoint),
});

// McpClient.CreateAsync performs the MCP initialize handshake:
//   client → { jsonrpc: "2.0", method: "initialize", params: { clientInfo, capabilities } }
//   server → { result: { serverInfo, capabilities, instructions } }
await using var client = await McpClient.CreateAsync(transport, new McpClientOptions
{
    ClientInfo = new Implementation { Name = "McpLearner", Version = "1.0.0" },
});

Console.WriteLine($"Server: {client.ServerInfo.Name} v{client.ServerInfo.Version}");
if (!string.IsNullOrEmpty(client.ServerInstructions))
    Console.WriteLine($"Instructions: {client.ServerInstructions}");
Console.WriteLine();

// ── Step 1: tools/list ─────────────────────────────────────────────────────────
// The SDK sends: { method: "tools/list" }
// The server replies with every registered tool's name, description, and JSON schema.
// The JSON schema is generated at startup from the C# method signatures + [Description] attrs.
Console.WriteLine("══════════ tools/list ══════════");
var tools = await client.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"  Name:   {tool.Name}");
    Console.WriteLine($"  Desc:   {tool.Description}");
    Console.WriteLine($"  Schema: {tool.JsonSchema.GetRawText()}");
    Console.WriteLine();
}

// ── Step 2: tools/call ─────────────────────────────────────────────────────────
// The SDK sends: { method: "tools/call", params: { name, arguments } }
// The server executes the matching [McpServerTool] method and serializes the return value
// into ContentBlock items. TextContentBlock carries the JSON-serialized return value.

Console.WriteLine("══════════ tools/call: get_weather ══════════");
await CallAndPrint(client, "get_weather", new() { ["city"] = "Sydney" });

Console.WriteLine("══════════ tools/call: add_numbers ══════════");
await CallAndPrint(client, "add_numbers", new() { ["a"] = 42.5, ["b"] = 7.5 });

Console.WriteLine("══════════ tools/call: multiply_numbers ══════════");
await CallAndPrint(client, "multiply_numbers", new() { ["a"] = 6.0, ["b"] = 7.0 });

Console.WriteLine("══════════ tools/call: get_time ══════════");
await CallAndPrint(client, "get_time", []);

Console.WriteLine();
Console.WriteLine("Done. Press any key to exit.");
Console.ReadKey(intercept: true);

static async Task CallAndPrint(McpClient client, string toolName, Dictionary<string, object?> arguments)
{
    CallToolResult result = await client.CallToolAsync(toolName, arguments);
    foreach (ContentBlock content in result.Content)
    {
        if (content is TextContentBlock text)
            Console.WriteLine($"  {text.Text}");
    }
    Console.WriteLine();
}
