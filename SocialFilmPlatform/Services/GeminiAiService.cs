using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace SocialFilmPlatform.Services
{
    public class GeminiAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

        public GeminiAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public async Task<(List<string> Tags, List<string> Categories)> SuggestTagsAndCategoriesAsync(string movieTitle, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    return (new List<string>(), new List<string>());
                }

                var prompt = $@"
Analyze the movie '{movieTitle}'. Description: '{description}'.
Suggest 3 relevant tags and 3 relevant categories for a bookmarking system.
Tags should be descriptive (e.g., 'Plot Twist', 'Inspring', 'Dark').
Categories should be broad genres or themes (e.g., 'Programming', 'Design', 'Sci-Fi').
Return ONLY a JSON object with this format:
{{
  ""tags"": [""tag1"", ""tag2"", ""tag3""],
  ""categories"": [""cat1"", ""cat2"", ""cat3""]
}}
";

                return await CallGeminiAsync(prompt);
            }
            catch (Exception)
            {
                return (new List<string>(), new List<string>());
            }
        }

        public async Task<(List<string> Tags, List<string> Categories)> SuggestTagsForListAsync(string listName, string listDescription, List<string> movieTitles)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    return (new List<string>(), new List<string>());
                }

                var moviesList = string.Join(", ", movieTitles.Take(30));
                var prompt = $@"
Analyze the following list of movies: {moviesList}.
List Name: '{listName}'.
List Description: '{listDescription}'.

Suggest 3 relevant tags and 3 relevant categories that describe this collection of movies as a whole.
Tags should be descriptive of the common theme (e.g., 'Mind Bending', '80s Classics', 'Tearjerkers').
Categories should be broad genres or themes (e.g., 'Sci-Fi', 'Comedy', 'Documentary').

Return ONLY a JSON object with this format:
{{
  ""tags"": [""tag1"", ""tag2"", ""tag3""],
  ""categories"": [""cat1"", ""cat2"", ""cat3""]
}}
";
                return await CallGeminiAsync(prompt);
            }
            catch (Exception)
            {
                return (new List<string>(), new List<string>());
            }
        }

        private async Task<(List<string> Tags, List<string> Categories)> CallGeminiAsync(string prompt)
        {
             var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ApiUrl}?key={_apiKey}", jsonContent);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Gemini API Error] Status: {response.StatusCode}");
                    Console.WriteLine($"[Gemini API Error] Body: {errorContent}");
                    return (new List<string>(), new List<string>());
                }

                var responseString = await response.Content.ReadAsStringAsync();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString, options);
                var text = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                if (string.IsNullOrEmpty(text))
                {
                    return (new List<string>(), new List<string>());
                }

                text = text.Replace("```json", "").Replace("```", "").Trim();

                var result = JsonSerializer.Deserialize<AiResult>(text, options);
                return (result?.Tags ?? new List<string>(), result?.Categories ?? new List<string>());
        }

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate> Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public List<Part> Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }

        private class AiResult
        {
            public List<string> Tags { get; set; }
            public List<string> Categories { get; set; }
        }
    }
}
