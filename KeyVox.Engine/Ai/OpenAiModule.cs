using KeyVox.Engine.Ai.Providers;
using KeyVox.Engine.SpeechRecognition;
using KeyVox.Engine.SpeechRecognition.SpeechToText;
using KeyVox.Engine.SpeechRecognition.SpeechToText.Providers;
using KeyVox.Engine.SpeechRecognition.TextToSpeech;
using KeyVox.Engine.SpeechRecognition.TextToSpeech.Providers;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Managers;

namespace KeyVox.Engine.Ai;

public static class OpenAiModule
{
    /// <summary>
    /// Injects <seealso cref="IOpenAiClient"/> 
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentException"></exception>
    static void AddOpenAiModule(this IServiceCollection services)
    {

        var apiKey = Environment.GetEnvironmentVariable("KEYVOX_AZ_AI_API_KEY") ?? 
                        throw new ArgumentException("Azure Open AI API Key not found");
        
        var openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = apiKey,
            ApiVersion = "2023-08-01-preview",
            DeploymentId = "gpt4-32",
            ResourceName = "redgate-ai",
            ProviderType = ProviderType.Azure
        });
        
        services.AddSingleton<IOpenAiClient>(_ => new AzureOpenAiClient(openAiService));
    }
}