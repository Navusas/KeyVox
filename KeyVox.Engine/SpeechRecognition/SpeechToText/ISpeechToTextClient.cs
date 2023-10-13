namespace KeyVox.Engine.SpeechRecognition.SpeechToText
{
    public interface ISpeechToTextClient
    {
        Task<string> OneShotRecognitionAsync();

        event Action<string> TextRecognized;
        Task StartRecognitionStreamAsync();
        Task<string> StopRecognitionStreamAsync();
    }
}