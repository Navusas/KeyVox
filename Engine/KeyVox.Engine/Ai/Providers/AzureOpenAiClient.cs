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
        var chatRequest = CreateRequest(snippet, request);

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
      
    public async IAsyncEnumerable<string> StreamChatAsync(string snippet, string request)
    {
        var chatRequest = CreateRequest(snippet, request);
        
        var completionResult = _client.ChatCompletion.CreateCompletionAsStream(chatRequest);

        await foreach (var completion in completionResult)
        {
            if (completion.Successful)
            {
                var funcCallResult = completion.Choices.FirstOrDefault()?.Message.FunctionCall?.Arguments;
                if (funcCallResult is null)
                {
                    yield return "Function wasn't called";
                    yield break;
                }

                yield return funcCallResult;
            }

            if (completion.Error == null) continue;
            yield return $"Error: [Code: {completion.Error.Code}]: {completion.Error.Message}";
            yield break;
        }
    }
    
    private static ChatCompletionCreateRequest CreateRequest(string snippet, string request)
    {
        return new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(Prompts.SystemPrompt),
                ChatMessage.FromUser(Prompts.UserPrompt(snippet, request))
            },
            Functions = new List<FunctionDefinition>{ Prompts.KeyVoxAssistantFunctionDefinition },
            FunctionCall = new Dictionary<string, string> { { "name", Prompts.FunctionName } },
            Model = "gpt-4-32k"
        };
    }
}