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

        event Action<string> TextRecognized;
        Task StartRecognitionStreamAsync();
        Task<string> StopRecognitionStreamAsync();
    }
}
