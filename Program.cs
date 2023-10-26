using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


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

        double similarityScore = await CalculateSimilarity(apiKey, text1, text2);

        Console.WriteLine($"Similarity Score: {similarityScore}");
    }

    static async Task<double> CalculateSimilarity(string apiKey, string text1, string text2)
    {
        string prompt = $"Compare the following two sentences:\n1. {text1}\n2. {text2}\nSimilarity:";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                prompt,
                max_tokens = 1,
                n = 1,
                stop = "\n"
            };

            string requestBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/engines/gpt-3.5-turbo-instruct/completions", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Extract the numeric value using a regular expression
                Match match = Regex.Match(responseContent, @"(\d+\.\d+)");

                if (match.Success)
                {
                    double similarityScore = double.Parse(match.Value);
                    return similarityScore;
                }
            }

            Console.WriteLine("Error: Unable to calculate similarity.");
            return 0.0;
        }
    }

    public class ApiResponse
    {
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public string text { get; set; }
    }
}

