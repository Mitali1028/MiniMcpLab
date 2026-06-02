namespace ToolServer.Tools;

using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
internal sealed class TimeTools
{
    [McpServerTool(Name = "get_time")]
    [Description("Get the current UTC date and time along with the Unix timestamp.")]
    public static TimeResult GetTime() => new(
        UtcNow: DateTime.UtcNow.ToString("O"),
        UnixTimestamp: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );
}

internal sealed record TimeResult(string UtcNow, long UnixTimestamp);
