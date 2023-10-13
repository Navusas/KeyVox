using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace KeyVox.Engine.SpeechRecognition.TextToSpeech.Providers
{
    public class AzureContinuousTextToSpeechClient : IDisposable
    {
        private SpeechSynthesizer _synthesizer;
        private readonly SpeechConfig _speechConfig;

        private readonly MemoryStream _audioStream;

        public AzureContinuousTextToSpeechClient(SpeechConfig speechConfig)
        {
            _speechConfig = speechConfig;
            _synthesizer = new SpeechSynthesizer(_speechConfig);
            _audioStream = new MemoryStream();
        }

        public async Task StartStream(string text, Action<Stream>? audioStreamGenerated)
        {
            var pullStream = AudioOutputStream.CreatePullStream();
            var audioConfig = AudioConfig.FromStreamOutput(pullStream);

            // Initialize the synthesizer with the new audioConfig
            _synthesizer = new SpeechSynthesizer(_speechConfig, audioConfig);

            _synthesizer.Synthesizing += (s, e) =>
            {
                _audioStream.Write(e.Result.AudioData, 0, e.Result.AudioData.Length);
                audioStreamGenerated?.Invoke(_audioStream);
            };
    
            await _synthesizer.SpeakTextAsync(text);
        }

        public Task<Stream> StopStream()
        {
            _audioStream.Position = 0;
            return Task.FromResult<Stream>(_audioStream);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _synthesizer.Dispose();
        }
    }
}