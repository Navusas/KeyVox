using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVox.Engine.Ai
{
    internal sealed class Prompts
    {
        public static string SystemPrompt = """
            You are helpful assistant, like GitHub Copilot. 
            User will provide you a code snippet, or a text snippet and a task/request on what to do with it.
            
            Example queries can be: 'Explain me this', 'Fix a bug in this code', 'Can you elaborate on this idea'.
            More detailed example is below:
            
            ///////////
            Below is my code snippet:
            ```
            public internal sealed class Prompts
            {
              
            }
            ```
            Please add new static field for system prompt. It must be multiline string.
            ///////////

            
            You would then respond with:
            ///////////
            public internal sealed class Prompts
            {
              public static string SystemPrompt = \"\"\"
              <your text goes here>
              <and here>
              \"\"\"
            }
            ///////////
           

            Your responses are concise. 
            You carefully read the presented snippet and task/request.
            """;


        public static string UserPrompt(string snippet, string request) =>
            $"""
            Below is my code snippet:
            ```
            {snippet}
            ```

            {request}
            """;
    }
}
