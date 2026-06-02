var builder = DistributedApplication.CreateBuilder(args);

var toolServer = builder.AddProject<Projects.ToolServer>("tool-server");

// WaitFor ensures McpClient doesn't start until ToolServer is healthy.
// WithReference injects the tool-server's URL into McpClient via environment variables:
//   services__tool-server__http__0=http://localhost:{PORT}
builder.AddProject<Projects.McpClient>("mcp-client")
    .WithReference(toolServer)
    .WaitFor(toolServer);

await builder.Build().RunAsync();
