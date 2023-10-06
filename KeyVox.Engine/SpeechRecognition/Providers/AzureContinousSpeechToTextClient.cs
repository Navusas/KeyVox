using System.Reactive.Subjects;
using System.Text;
using Microsoft.CognitiveServices.Speech;

namespace KeyVox.Engine.SpeechRecognition.Providers
{
    public class AzureContinousSpeechToTextClient : IDisposable
    {

        private SpeechRecognizer _recognizer;
        private StringBuilder _finalResultBuilder;
        private MemoryStream _memoryStream;
        private StreamWriter _streamWriter;

        public AzureContinousSpeechToTextClient(SpeechConfig speechConfig)
        {
            _recognizer = new SpeechRecognizer(speechConfig);
            _finalResultBuilder = new();
            _memoryStream = new();
            _streamWriter = new(_memoryStream);

        }


        public async Task<Stream> StartStream()
        {

            _recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    _finalResultBuilder.Append(e.Result.Text + " ");
                }
            };
            _recognizer.Recognizing += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizingSpeech)
                {
                    Console.WriteLine(e.Result.Text);
                    if (_memoryStream != null && _memoryStream.CanWrite)
                    {
                        _streamWriter.Write(e.Result.Text + " ");
                        _streamWriter.Flush();
                    }


                }
            };
            await _recognizer!.StartContinuousRecognitionAsync();

            _memoryStream.Position = 0;
            return _memoryStream;

        }

        public async Task<string> StopStream()
        {
            await _recognizer.StopContinuousRecognitionAsync();
            return _finalResultBuilder.ToString();
        }

        public async void Dispose()
        {
            await _recognizer.StopContinuousRecognitionAsync();
            _recognizer.Dispose();

            _streamWriter?.Dispose();
            _memoryStream?.Dispose();
        }
    }
}
