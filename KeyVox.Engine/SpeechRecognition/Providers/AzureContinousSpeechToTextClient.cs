using System.Reactive.Subjects;
using System.Text;
using Microsoft.CognitiveServices.Speech;

namespace KeyVox.Engine.SpeechRecognition.Providers
{
    public class AzureContinousSpeechToTextClient : IDisposable
    {
        private SpeechRecognizer _recognizer;
        private StringBuilder _finalResultBuilder;

        public AzureContinousSpeechToTextClient(SpeechConfig speechConfig)
        {
            _recognizer = new SpeechRecognizer(speechConfig);
            _finalResultBuilder = new();
        }


        public async Task StartStream(Action<string> TextRecognized)
        {
            _recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    _finalResultBuilder.Append(e.Result.Text + " ");
                    TextRecognized?.Invoke(_finalResultBuilder.ToString());
                }
            };
            _recognizer.Recognizing += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizingSpeech)
                {
                    TextRecognized?.Invoke(_finalResultBuilder.ToString() + e.Result.Text);
                }
            };
            await _recognizer!.StartContinuousRecognitionAsync();
        }

        public async Task<string> StopStream()
        {
            await _recognizer.StopContinuousRecognitionAsync();
            return _finalResultBuilder.ToString();
        }

        public async void Dispose()
        {
            Console.WriteLine("Disposing resources.");
            GC.SuppressFinalize(this);
            await _recognizer.StopContinuousRecognitionAsync();
            _recognizer.Dispose();
        }
    }
}