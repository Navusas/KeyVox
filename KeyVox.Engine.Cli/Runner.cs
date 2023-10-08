﻿using System.Text.Json;
using KeyVox.Engine.Ai;
using KeyVox.Engine.Ai.Providers;
using KeyVox.Engine.SpeechRecognition;
using KeyVox.Engine.SpeechRecognition.Providers;

namespace KeyVox.Engine.Cli;

public static class Runner
{
    public static async Task Start(string userQuery)
    {
        IOpenAiClient openAi = new AzureOpenAiClient();
        ISpeechToTextClient speechToTextClient = new AzureSpeechToTextClient();

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
        
        var result = await openAi.ChatAsync(userQuery,finalizedSpeechResult);
        var functionCallArgs = JsonSerializer.Deserialize<Prompts.AssistantResponseFuncArgs>(result);
        if (functionCallArgs?.Comments is not null)
        {
            Console.WriteLine($"[KeyVox]: AI comments:\n############################\n{functionCallArgs?.Comments}\n############################");
        }
        Console.WriteLine($"[KeyVox]: AI snippet response:\n############################\n{functionCallArgs?.ResponseSnippet}\n############################");
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
}