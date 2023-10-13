using OpenAI;
using OpenAI.Chat;

namespace KeyVox.Engine.Ai.Providers;

public class AzureOpenAiClient : IOpenAiClient
{
    private readonly string _azureApikey;
    private OpenAIClient _client;

    public AzureOpenAiClient(string azureApikey)
    {
        _azureApikey = azureApikey;
    }

    public async Task<string> ChatAsync(string snippet, string request)
    {
        var auth = new OpenAIAuthentication(_azureApikey);
        var settings = new OpenAIClientSettings(
            resourceName: "redgate-ai",
            deploymentId: "gpt4-32",
            apiVersion: "2023-08-01-preview");
        _client = new OpenAIClient(auth, settings);

        var messages = new List<Message>
        {
            new(Role.System, Prompts.SystemPrompt),
            new(Role.User, Prompts.UserPrompt(snippet, request))
        };

        try
        {
            var chatRequest = new ChatRequest(messages,
                functions: new[] { Prompts.KeyVoxAssistantFunction },
                functionCall: Prompts.FunctionName,
                model: "gpt-4-32k");

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