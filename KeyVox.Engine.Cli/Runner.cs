using System.Text.Json;
using KeyVox.Engine.Ai;
using KeyVox.Engine.Ai.Providers;
using KeyVox.Engine.SpeechRecognition.SpeechToText;
using KeyVox.Engine.SpeechRecognition.SpeechToText.Providers;

namespace KeyVox.Engine.Cli;

public static class Runner
{
    public static async Task Start(string userQuery)
    {
        var azureOpenAiApiKey = GetEnvVariableOrThrow("KEYVOX_AZ_AI_API_KEY");
        var speechRecognitionApiKey = GetEnvVariableOrThrow("KEYVOX_AZ_SPEECH_RECOGNITION_API_KEY");
        var speechRecognitionRegion = GetEnvVariableOrThrow("KEYVOX_AZ_SPEECH_RECOGNITION_REGION");

        IOpenAiClient openAi = new AzureOpenAiClient(azureOpenAiApiKey);
        ISpeechToTextClient speechToTextClient =
            new AzureSpeechToTextClient(speechRecognitionApiKey, speechRecognitionRegion);

        Console.WriteLine("[KeyVox]: What's your query? (speak into the microphone)...\n");
        Console.WriteLine("[KeyVox]: Press any key to stop recording...");

        // Subscribe to the event
        speechToTextClient.TextRecognized += (text) =>
        {
            UpdateLastLine(text);
            Console.WriteLine("[KeyVox]: Press any key to stop recording...");
        };

        // Start speech recognition
        await speechToTextClient.StartRecognitionStreamAsync();
        Console.ReadKey();

        // Stop recognition
        var finalizedSpeechResult = await speechToTextClient.StopRecognitionStreamAsync();
        Console.WriteLine($"[KeyVox]: You asked: '{finalizedSpeechResult}'");
        Console.WriteLine("[KeyVox]: Asking AI...");

        var result = await openAi.ChatAsync(userQuery, finalizedSpeechResult);
        try
        {
            var functionCallArgs = JsonSerializer.Deserialize<Prompts.AssistantResponseFuncArgs>(result);
            if (functionCallArgs?.Context is not null)
            {
                Console.WriteLine(
                    $"[KeyVox]: AI comments:\n############################\n{functionCallArgs?.Context}\n############################");
            }

            Console.WriteLine(
                $"[KeyVox]: AI snippet response:\n############################\n{functionCallArgs?.ResponseSnippet}\n############################");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            Console.WriteLine($"Problematic JSON: {result}");
        }
    }

    private static void UpdateLastLine(string newText)
    {
        // Move the cursor to the beginning of the last line
        Console.SetCursorPosition(0, Console.CursorTop - 1);

        // Clear the current line
        Console.Write(new string(' ', Console.WindowWidth));

        // Reset the cursor position again to the beginning of the last line
        Console.SetCursorPosition(0, Console.CursorTop - 1);

        // Write the new text
        Console.WriteLine($"[KeyVox]: We heard: '{newText}'");
    }

    private static string GetEnvVariableOrThrow(string parameterName)
        => Environment.GetEnvironmentVariable(parameterName) ??
           throw new ArgumentException($"{parameterName} not set and not found");
}
