using KeyVox.Engine.SpeechRecognition.SpeechToText;
using KeyVox.Engine.SpeechRecognition.TextToSpeech;

namespace KeyVox.Engine.SpeechRecognition;

public class SpeechClient
{
    public ISpeechToTextClient SpeechToText { get; }
    public ITextToSpeechClient TextToSpeech { get; }

    public SpeechClient(ISpeechToTextClient sttClient, ITextToSpeechClient ttsClient)
    {
        SpeechToText = sttClient;
        TextToSpeech = ttsClient;
    }
}