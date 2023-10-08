using CommandLine;

namespace KeyVox.Engine.Cli;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("[KeyVox]: Hello to KeyVox!");

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunWithOptions)
            .WithNotParsed(HandleParseError);
    }

    private static void RunWithOptions(Options opts)
    {
        Console.WriteLine($"[KeyVox]: Copied context:\n############################\n{opts.UserQuery}\n############################");
        Task.Run(() => Runner.Start(opts.UserQuery)).Wait();
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors (if any)
        Console.WriteLine("Error parsing arguments.");
        Environment.Exit(1);
    }   
}