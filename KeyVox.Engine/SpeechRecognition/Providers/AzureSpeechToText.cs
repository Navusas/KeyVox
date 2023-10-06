using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyVox.Engine.SpeechRecognition;
using Microsoft.CognitiveServices.Speech;

namespace KeyVox.Engine.SpeechRecognition.Providers
{
    internal class AzureSpeechToText : ISpeechToText
    {
        private readonly string _apiKey;
        private readonly string _region;

        public AzureSpeechToText()
        {
            _apiKey = Environment.GetEnvironmentVariable("KEYVOX_AZ_S2T_API_KEY") ?? throw new ArgumentNullException(nameof(_apiKey));
            _region = Environment.GetEnvironmentVariable("KEYVOX_AZ_S2T_REGION") ?? throw new ArgumentNullException(nameof(_region));
        }

        public async Task<string> RecognizeSpeechAsync()
        {
            var speechConfig = SpeechConfig.FromSubscription(_apiKey, _region);
            using var recognizer = new SpeechRecognizer(speechConfig);

            Console.WriteLine("Listening for speech...");
            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"Recognized: {result.Text}");
                return result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine("No speech could be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");
            }
            return string.Empty;
        }
    }
}
