namespace ToolServer.Tools;

using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
internal sealed class MathTools
{
    [McpServerTool(Name = "add_numbers")]
    [Description("Add two numbers and return their sum.")]
    public static double AddNumbers(
        [Description("The first number")] double a,
        [Description("The second number")] double b) => a + b;

    [McpServerTool(Name = "multiply_numbers")]
    [Description("Multiply two numbers and return their product.")]
    public static double MultiplyNumbers(
        [Description("The first number")] double a,
        [Description("The second number")] double b) => a * b;
}
