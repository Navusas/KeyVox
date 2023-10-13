using Microsoft.CognitiveServices.Speech;

namespace KeyVox.Engine.SpeechRecognition.SpeechToText.Providers
{
    public sealed class AzureSpeechToTextClient : ISpeechToTextClient
    {
        private readonly string _apiKey;
        private readonly string _region;
        private AzureContinousSpeechToTextClient? _continuousSpeechToTextClient;

        public event Action<string>? TextRecognized;

        public AzureSpeechToTextClient(string apiKey, string region)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _region = region ?? throw new ArgumentNullException(nameof(region));
        }

        /// <summary>
        /// This just listens to the recognition once, and then when there is pause in the voice
        /// stops and returns the response
        /// </summary>
        /// <returns></returns>
        public async Task<string> OneShotRecognitionAsync()
        {
            var speechConfig = SpeechConfig.FromSubscription(_apiKey, _region);
            var recognizer = new SpeechRecognizer(speechConfig);

            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                return result.Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// This one starts 'continous' speech recognition,
        /// and continues to do so until <seealso cref="StopRecognitionStreamAsync"/> is invoked
        /// </summary>
        /// <returns></returns>
        public async Task StartRecognitionStreamAsync()
        {
            var speechConfig = SpeechConfig.FromSubscription(_apiKey, _region);
            _continuousSpeechToTextClient = new AzureContinousSpeechToTextClient(speechConfig);

            await _continuousSpeechToTextClient.StartStream(TextRecognized);
        }

        public async Task<string> StopRecognitionStreamAsync()
        {
            if (_continuousSpeechToTextClient is not null)
            {
                var result = await _continuousSpeechToTextClient.StopStream();
                _continuousSpeechToTextClient.Dispose();
                return result;
            }
            else return string.Empty;
        }
    }
}