namespace ToolServer.Tools;

using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
internal sealed class WeatherTools
{
    private static readonly string[] Conditions = ["Sunny", "Cloudy", "Rainy", "Windy", "Stormy"];

    [McpServerTool(Name = "get_weather")]
    [Description("Get the current weather for a city. Returns temperature, condition, and humidity.")]
    public static WeatherResult GetWeather(
        [Description("The name of the city to get weather for, e.g. 'Sydney' or 'London'")] string city)
    {
        // Deterministic mock — same city always returns the same result so it's easy to verify.
        var random = new Random(city.ToLowerInvariant().GetHashCode());
        return new WeatherResult(
            City: city,
            TemperatureCelsius: random.Next(5, 38),
            Condition: Conditions[random.Next(Conditions.Length)],
            HumidityPercent: random.Next(30, 90)
        );
    }
}

internal sealed record WeatherResult(string City, int TemperatureCelsius, string Condition, int HumidityPercent);
