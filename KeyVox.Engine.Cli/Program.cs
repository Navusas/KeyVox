﻿using KeyVox.Engine.SpeechRecognition;
using KeyVox.Engine.SpeechRecognition.Providers;

Console.WriteLine("Hello!");


//var speechService = new AzureSpeechToTextClient();

//Console.WriteLine("Press any key to start listening...");
//Console.ReadKey();

//string transcribedText = await speechService.RecognizeSpeechAsync();
//Console.WriteLine($"Transcribed Text: {transcribedText}");

//Console.WriteLine("Press any key to exit...");
//Console.ReadKey();



ISpeechToTextClient speechToTextClient = new AzureSpeechToTextClient();

// Subscribe to the event
speechToTextClient.TextRecognized += recognizedText =>
{
    Console.WriteLine(recognizedText); // Print the recognized text as it comes in
};

// Start recognition
await speechToTextClient.StartRecognitionStreamAsync();

Console.WriteLine("Press any key to stop...");
Console.ReadKey();

// Stop recognition
var finalResult = await speechToTextClient.StopRecognitionStreamAsync();
Console.WriteLine("\nFinal Result: " + finalResult);

// Unsubscribe from the event to clean up
speechToTextClient.TextRecognized -= recognizedText =>
{
    Console.WriteLine(recognizedText);
};