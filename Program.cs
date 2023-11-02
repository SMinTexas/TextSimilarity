using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

class Program
{
    static async Task Main()
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key is missing.  Make sure to set the OPENAI_API_KEY environment variable.");
            return;
        }
        
        Console.WriteLine("Text Similarity Checker");

        Console.Write("Enter the first text: ");
        string text1 = Console.ReadLine();

        Console.Write("Enter the second text: ");
        string text2 = Console.ReadLine();

        var similarityScore = await CalculateSimilarity(apiKey, text1, text2);

        Console.WriteLine($"Similarity Score: {similarityScore}");
    }

    static async Task<decimal> CalculateSimilarity(string apiKey, string text1, string text2)
    {
        decimal result = 0.0m;
        string prompt = $"Compare the following two sentences:\n1. {text1}\n2. {text2}\nSimilarity:";

        Request request = new Request
        {
            Messages = new RequestMessage[]
            {
                new RequestMessage
                {
                    Role = "system",
                    Content = "You are a text comparison assistant. You will return a ranking from 1 to 5 when comparing text. 1 will be the text is not similar. A response of 5 will mean the text is identical.",
                },
                new RequestMessage
                {
                    Role = "user",
                    Content = prompt,
                },
            }
        };
        string requestJson = JsonSerializer.Serialize(request);

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            // var requestBody = new
            // {
            //     prompt,
            //     max_tokens = 10,
            //     n = 1,
            //     // stop = "\n"
            // };

        //     string requestBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (httpResponse.IsSuccessStatusCode)
            {
                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<Response>(responseContent);
                var ranking = response.Choices[0].Message.Content;
                if (decimal.TryParse(ranking, out result))
                {
                    return result;
                }
            }
        }

        Console.WriteLine("Error: Unable to calculate similarity.");
        return result;
    }

    public class ApiResponse
    {
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public string text { get; set; }
    }

     public class Request
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "gpt-3.5-turbo";
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 4000;
        [JsonPropertyName("messages")]
        public RequestMessage[] Messages { get; set; }
    }

    public class RequestMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("created")]
        public int Created { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("usage")]
        public ResponseUsage Usage { get; set; }
        [JsonPropertyName("choices")]
        public ResponseChoice[] Choices { get; set; }
    }

    public class ResponseUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class ResponseChoice
    {
        [JsonPropertyName("message")]
        public ResponseMessage Message { get; set; }
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    public class ResponseMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}

