using KeyVox.Engine.SpeechRecognition.SpeechToText;
using KeyVox.Engine.SpeechRecognition.TextToSpeech;

namespace KeyVox.Engine.SpeechRecognition;

public class SpeechRecognitionClient
{
    public ISpeechToTextClient SpeechToText { get; }
    public ITextToSpeechClient TextToSpeech { get; }

    public SpeechRecognitionClient(ISpeechToTextClient sttClient, ITextToSpeechClient ttsClient)
    {
        SpeechToText = sttClient;
        TextToSpeech = ttsClient;
    }
}