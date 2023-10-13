using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;

namespace KeyVox.Engine.Ai.Providers;

public class AzureOpenAiClient : IOpenAiClient
{
    private readonly OpenAIService _client;

    public AzureOpenAiClient(OpenAIService openAiService)
    {
        _client = openAiService;
    }

    public async Task<string> ChatAsync(string snippet, string request)
    {
        var chatRequest = new ChatCompletionCreateRequest()
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(Prompts.SystemPrompt),
                ChatMessage.FromUser(Prompts.UserPrompt(snippet, request))
            },
            Model = "gpt-4-32k"
        };

        try
        {
            var completionResult = await _client.ChatCompletion.CreateCompletion(chatRequest);
            
            if (completionResult.Successful)
            {
                var funcCallResult = completionResult.Choices.FirstOrDefault()?.Message.FunctionCall?.Arguments;
                if (funcCallResult is null)
                {
                    throw new Exception("Function wasn't called");
                }

                return funcCallResult;
            }

            if (completionResult.Error == null)
            {
                throw new Exception("Unknown Error");
            }

            return $"Error: [Code: {completionResult.Error.Code}]: {completionResult.Error.Message}";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}