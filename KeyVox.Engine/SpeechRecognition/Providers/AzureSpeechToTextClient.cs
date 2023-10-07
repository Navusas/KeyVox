using System.Reactive.Subjects;
using System.Text;
using Microsoft.CognitiveServices.Speech;

namespace KeyVox.Engine.SpeechRecognition.Providers
{
    public sealed class AzureSpeechToTextClient : ISpeechToTextClient
    {
        private readonly string _apiKey;
        private readonly string _region;
        private AzureContinousSpeechToTextClient? _continousSpeechToTextClient;

        public event Action<string> TextRecognized;

        public AzureSpeechToTextClient()
        {
            _apiKey = Environment.GetEnvironmentVariable("KEYVOX_AZ_S2T_API_KEY") ??
                      throw new ArgumentNullException(nameof(_apiKey));
            _region = Environment.GetEnvironmentVariable("KEYVOX_AZ_S2T_REGION") ??
                      throw new ArgumentNullException(nameof(_region));
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
            _continousSpeechToTextClient = new AzureContinousSpeechToTextClient(speechConfig);

            await _continousSpeechToTextClient.StartStream(TextRecognized);
        }

        public async Task<string> StopRecognitionStreamAsync()
        {
            if (_continousSpeechToTextClient is not null)
            {
                var result = await _continousSpeechToTextClient.StopStream();
                _continousSpeechToTextClient.Dispose();
                return result;
            }
            else return string.Empty;
        }
    }
}