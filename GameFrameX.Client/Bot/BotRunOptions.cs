namespace GameFrameX.Client.Bot;

/// <summary>
/// 机器人运行参数。
/// </summary>
public sealed class BotRunOptions
{
    public int BotCount { get; init; } = 50;
    public string BotNamePrefix { get; init; } = "BotClient";
    public string TcpHost { get; init; } = "127.0.0.1";
    public int TcpPort { get; init; } = 49100;
    public string LoginUrl { get; init; } = "http://127.0.0.1:48080/game/api/";
    public int ConnectStaggerMilliseconds { get; init; } = 20;
    public bool EnableDisconnectLoop { get; init; } = true;
    public int DisconnectAfterLoginSeconds { get; init; } = 15;
    public int RunSeconds { get; init; } = 0;

    public static BotRunOptions Parse(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var arg in args)
        {
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var segment = arg.Substring(2);
            var splitIndex = segment.IndexOf('=');
            if (splitIndex <= 0 || splitIndex == segment.Length - 1)
            {
                continue;
            }

            var key = segment.Substring(0, splitIndex).Trim();
            var value = segment.Substring(splitIndex + 1).Trim();
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                values[key] = value;
            }
        }

        return new BotRunOptions
        {
            BotCount = ReadInt(values, "bot-count", 50),
            BotNamePrefix = ReadString(values, "bot-prefix", "BotClient"),
            TcpHost = ReadString(values, "tcp-host", "127.0.0.1"),
            TcpPort = ReadInt(values, "tcp-port", 49100),
            LoginUrl = EnsureEndWithSlash(ReadString(values, "login-url", "http://127.0.0.1:48080/game/api/")),
            ConnectStaggerMilliseconds = ReadInt(values, "connect-stagger-ms", 20),
            EnableDisconnectLoop = ReadBool(values, "disconnect-loop", true),
            DisconnectAfterLoginSeconds = ReadInt(values, "disconnect-after-login-seconds", 15),
            RunSeconds = ReadInt(values, "run-seconds", 0),
        };
    }

    private static string ReadString(IDictionary<string, string> values, string key, string defaultValue)
    {
        return values.TryGetValue(key, out var value) ? value : defaultValue;
    }

    private static int ReadInt(IDictionary<string, string> values, string key, int defaultValue)
    {
        if (!values.TryGetValue(key, out var value))
        {
            return defaultValue;
        }

        return int.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    private static bool ReadBool(IDictionary<string, string> values, string key, bool defaultValue)
    {
        if (!values.TryGetValue(key, out var value))
        {
            return defaultValue;
        }

        return bool.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    private static string EnsureEndWithSlash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "http://127.0.0.1:48080/game/api/";
        }

        return value.EndsWith("/", StringComparison.Ordinal) ? value : $"{value}/";
    }
}
