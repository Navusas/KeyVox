
namespace KeyVox.Engine.Ai;

public interface IOpenAiClient
{
    Task<string> ChatAsync(string snippet, string request);
}
