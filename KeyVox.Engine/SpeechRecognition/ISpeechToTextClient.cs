using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVox.Engine.SpeechRecognition
{
    public interface ISpeechToTextClient
    {
        Task<string> OneShotRecognitionAsync();

        Task<Stream> StartRecognitionStreamAsync();
        Task<string> StopRecognitionStreamAsync();
    }
}
