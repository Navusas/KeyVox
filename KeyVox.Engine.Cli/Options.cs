namespace KeyVox.Engine.Cli;

using CommandLine;

public class Options
{
    [Option('q', "userQuery", Required = true, HelpText = "User query to process.")]
    public string UserQuery { get; set; }
}