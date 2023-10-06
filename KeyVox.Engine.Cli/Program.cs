using KeyVox.Engine.SpeechRecognition;
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
var streamReader = await speechToTextClient.StartRecognitionStreamAsync();

using (var reader = new StreamReader(streamReader))
{
    var responseAsString = await reader.ReadToEndAsync();
    Console.WriteLine(responseAsString);
}

Console.WriteLine("Press any key to stop...");
Console.ReadKey();

var finalResult = await speechToTextClient.StopRecognitionStreamAsync();
Console.WriteLine("\nFinal Result: " + finalResult);