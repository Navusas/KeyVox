using KeyVox.Engine.SpeechRecognition.SpeechToText;
using KeyVox.Engine.SpeechRecognition.SpeechToText.Providers;
using KeyVox.Engine.SpeechRecognition.TextToSpeech;
using KeyVox.Engine.SpeechRecognition.TextToSpeech.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace KeyVox.Engine.SpeechRecognition;

public static class SpeechRecognitionModule
{
    /// <summary>
    /// Injects <seealso cref="ISpeechToTextClient"/> and <seealso cref="ITextToSpeechClient"/> alongside their compositor
    /// <seealso cref="SpeechRecognitionClient"/>
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentException"></exception>
    static void AddSpeechRecognition(this IServiceCollection services)
    {
        var apiKey = Environment.GetEnvironmentVariable("KEYVOX_AZ_SPEECH_RECOGNITION_API_KEY") ?? 
                        throw new ArgumentException("Speech Recognition API Key not found");
        var region = Environment.GetEnvironmentVariable("KEYVOX_AZ_SPEECH_RECOGNITION_REGION") ?? 
                        throw new ArgumentException("Speech Recognition  Region not found");

        services.AddSingleton<ISpeechToTextClient>(_ => new AzureSpeechToTextClient(apiKey, region));
        services.AddSingleton<ITextToSpeechClient>(_ => new AzureTextToSpeechClient(apiKey, region));

        services.AddSingleton<SpeechRecognitionClient>();
    }
}