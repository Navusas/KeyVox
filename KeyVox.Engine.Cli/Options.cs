namespace KeyVox.Engine.Cli;

using CommandLine;

public class Options
{
    [Option('f', "contextFile", Required = true, HelpText = "File containing user selected context to process.")]
    public string ContextFile { get; set; }
}