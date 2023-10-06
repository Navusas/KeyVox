
using OpenAI;
using OpenAI.Chat;

namespace KeyVox.Engine.Ai.Providers;

public class AzureOpenAiClient : IOpenAiClient
{
    private readonly string _uri;
    private readonly string _apiKey;
    private OpenAIClient _client;
    public AzureOpenAiClient()
    {
        _uri = Environment.GetEnvironmentVariable("KEYVOX_AZ_AI_URL") ?? throw new ArgumentNullException(nameof(_uri));
        _apiKey = Environment.GetEnvironmentVariable("KEYVOX_AZ_AI_API_KEY") ?? throw new ArgumentNullException(nameof(_apiKey));
    }

    public async Task<string> ChatAsync(string snippet, string request)
    {
        var auth = new OpenAIAuthentication(_apiKey);
        var settings = new OpenAIClientSettings(
            resourceName: "redgate-ai",
            deploymentId: "gpt35-16k",
            apiVersion: "2023-08-01-preview");
        _client = new OpenAIClient(auth, settings);


        var messages = new List<Message>
    {
        new Message(Role.System,Prompts.SystemPrompt),
        new Message(Role.User, Prompts.UserPrompt(snippet, request))
    };


        try
        {
            var chatRequest = new ChatRequest(messages,
                functions: new[] { Prompts.KeyVoxAssistantFunction },
                functionCall: Prompts.FunctionName,
                model: "gpt-35-turbo-16k");

            var result = await _client.ChatEndpoint.GetCompletionAsync(chatRequest);
            var funcCallResult = result.FirstChoice.Message.Function.Arguments.ToString();
            return funcCallResult;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }
}