using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVox.Engine.SpeechRecognition
{
    internal interface ISpeechToText
    {
        Task<string> RecognizeSpeechAsync();
    }
}
