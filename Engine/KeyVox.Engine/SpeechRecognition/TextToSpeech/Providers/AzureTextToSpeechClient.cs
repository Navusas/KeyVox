using Microsoft.CognitiveServices.Speech;

namespace KeyVox.Engine.SpeechRecognition.TextToSpeech.Providers;

    public sealed class AzureTextToSpeechClient : ITextToSpeechClient
    {
        private readonly string _apiKey;
        private readonly string _region;
        private AzureContinuousTextToSpeechClient? _continuousTextToSpeechClient;

        public event Action<Stream>? AudioStreamGenerated;

        public AzureTextToSpeechClient(string apiKey, string region)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _region = region ?? throw new ArgumentNullException(nameof(region));
        }

        public async Task<Stream> OneShotSpeechAsync(string text)
        {
            var speechConfig = SpeechConfig.FromSubscription(_apiKey, _region);
            using var synthesizer = new SpeechSynthesizer(speechConfig);
            var result = await synthesizer.SpeakTextAsync(text);
            
            return result.Reason == ResultReason.SynthesizingAudioCompleted 
                ? new MemoryStream(result.AudioData) 
                : new MemoryStream();
        }

        public async Task StartSpeechStreamAsync(string text)
        {
            var speechConfig = SpeechConfig.FromSubscription(_apiKey, _region);
            _continuousTextToSpeechClient = new AzureContinuousTextToSpeechClient(speechConfig);

            await _continuousTextToSpeechClient.StartStream(text, AudioStreamGenerated);
        }

        public async Task<Stream> StopSpeechStreamAsync()
        {
            if (_continuousTextToSpeechClient is null) return new MemoryStream();
            var result = await _continuousTextToSpeechClient.StopStream();
            _continuousTextToSpeechClient.Dispose();
            return result;
        }
    }