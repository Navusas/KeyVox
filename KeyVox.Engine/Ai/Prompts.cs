using System.Text.Json;
using OpenAI.Chat;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace KeyVox.Engine.Ai;

public static class Prompts
{
    // You are a code transformation assistant. When given code or text and a specific request, transform or explain it. Prioritize clear, direct action based on user requests.

    public const string SystemPrompt =
        """
            You are a programming and content transformation assistant.
            Users will provide a snippet of code or text and a specific request about it.
            Always prioritize direct actionable changes based on their request. If they ask for conversion, transform the content.
            If any request feels ambiguous, use your expertise to make educated decisions but always align with the user's intent.
            Remember: Action is better than inaction when a task is provided.
        """;


    public static string UserPrompt(string snippet, string request)
    {
        var serializedSnippet = JsonSerializer.Serialize(snippet);

        return 
            $"""
             User has provided a snippet and has the following request:
             >>> {request} <<<

             Here is the original snippet (serialized as JSON):
             >>>
             {serializedSnippet}
             <<<

             Please act on the request and provide an actionable response.
             """;
    }

    public static readonly string FunctionName = "assistant_response";

    public static readonly Function KeyVoxAssistantFunction = new(
        FunctionName,
        """
        Respond to the user's request pertaining to the provided snippet. Adhere to the following guidelines:
            - Always make actionable changes if the user's request specifies them.
            - If a request is unactioned, provide a strong justification in the 'context'.
            - If the user's request demands an update or alteration to the snippet, make the necessary changes and populate the 'snippet' property.
            - If the snippet remains unchanged or if there's a reason to retain the original form, explain the decision in the 'context' property.
            - Always provide a reason or rationale in the 'context' for the choices made, especially when no modifications are performed.
            - Ensure the response is accurate, concise, and directly relevant to the user's request.
            - When in doubt or when faced with ambiguities, make educated assumptions but clearly indicate them in the 'context'.
            - Prioritize clarity and user comprehension in all responses.
        """,
        new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["snippet"] =
                    new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "The actioned or altered version of the snippet based on the user's request."

                    },
                ["context"] = new JsonObject
                {
                    ["type"] = "string",
                    ["description"] = "Explanatory remarks, clarifications, or reasons for the decisions made during snippet modifications. This is also where ambiguities or assumptions are noted."
                },
            },
            ["required"] = new JsonArray { "snippet" }
        });

    public class AssistantResponseFuncArgs
    {
        [JsonPropertyName("snippet")] public string ResponseSnippet { get; set; }

        [JsonPropertyName("context")] public string? Context { get; set; }
    }

}
