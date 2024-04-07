using CommandLine;

namespace GameFrameX.Launcher.Common.Options;

public sealed class LauncherOptions
{
    [Option('s', "ServerType", HelpText = "Enter the type of server to start")]
    public string ServerType { get; set; }
}