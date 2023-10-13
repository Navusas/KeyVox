namespace KeyVox.Engine.SpeechRecognition.TextToSpeech;

public interface ITextToSpeechClient
{
    Task<Stream> OneShotSpeechAsync(string text);
        
    event Action<Stream> AudioStreamGenerated;
    Task StartSpeechStreamAsync(string text);
    Task<Stream> StopSpeechStreamAsync();
}