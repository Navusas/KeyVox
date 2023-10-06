using Azure;
using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVox.Engine.Ai.Providers
{
    public class AzureOpenAiClient : IOpenAiClient
    {
        private readonly string _deploymentName = "gpt35-16k";
        private readonly string _uri;
        private readonly string _apiKey;
        private OpenAIClient _client;
        public AzureOpenAiClient()
        {
            _uri = Environment.GetEnvironmentVariable("KEYVOX_AZ_AI_URL") ?? throw new ArgumentNullException(nameof(_uri));
            _apiKey = Environment.GetEnvironmentVariable("KEYVOX_AZ_AI_API_KEY") ?? throw new ArgumentNullException(nameof(_apiKey));
        }

        //public Task<Completions> ChatGptAsync(Completions[] messages, OpenAiFunction[] functions = null, string model = null, string functionCall = "auto")
    }
}
