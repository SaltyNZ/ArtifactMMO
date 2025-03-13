using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Spectre.Console;
using Spectre.Console.Json;

namespace ArtifactMMO
{
    public class ArtifactApiService
    {
        private readonly HttpClient _client;
        UI ui = new UI();
        bool debug = false;
        public ArtifactApiService()
        {
            _client = new HttpClient();
        }

        public async Task<T?> PerformActionAsync<T>(string? characterName, string token, string action, object requestBody, string progressMessage = "Cooldown is: ") where T : class
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/{action}";
            
            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            T? apiResponse = await HandlePostResponse<T>(response);

            if (apiResponse is IHasCooldown cooldownResponse && cooldownResponse.Cooldown is not null)
            {
                int waitTime = (int)(cooldownResponse.Cooldown.Expiration - cooldownResponse.Cooldown.StartedAt).TotalSeconds;
                
                if (waitTime > 0) await ShowProgressBar(progressMessage, waitTime + 1);
            }

            return apiResponse;

        }

        public async Task<TResponse?> HandlePostResponse<TResponse>(HttpResponseMessage response) where TResponse : class
        {
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(result);

                string formattedResult = JsonSerializer.Serialize(JsonDocument.Parse(result), new JsonSerializerOptions
                {
                    WriteIndented = true // Adds line breaks & indentation
                });

                if(debug == true)
                {
                    AnsiConsole.Write(
                    new Panel(Markup.Escape(formattedResult))
                        .Header("Response JSON")
                        .Expand()
                        .RoundedBorder()
                        .BorderColor(Spectre.Console.Color.Blue)
                    );
                }
                
                return apiResponse?.Data;
                

            }
            else
            {
                Console.WriteLine($"Error-01: {response.StatusCode}");
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw Response: {result}");

                return null;
            }
        }

        public async Task<TResponse?> HandleGetResponse<TResponse>(HttpResponseMessage response) where TResponse : class
        {
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(result);

                string formattedResult = JsonSerializer.Serialize(JsonDocument.Parse(result), new JsonSerializerOptions
                {
                    WriteIndented = true // Adds line breaks & indentation
                });

                if(debug == true)
                {
                    AnsiConsole.Write(
                    new Panel(Markup.Escape(formattedResult))
                        .Header("Response JSON")
                        .Expand()
                        .RoundedBorder()
                        .BorderColor(Spectre.Console.Color.Blue)
                    );
                }
                
                return apiResponse?.Data;
               
            }
            else
            {
                Console.WriteLine($"Error-01: {response.StatusCode}");
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw Response: {result}");
                return null;
            }
        }

        // ===============================
        // Progress Bar Methods
        // ===============================
        public static async Task ShowProgressBar(string taskName, int waitTime)
        {
            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask(taskName);
                    task.MaxValue(100);
                    
                    int interval = 100;
                    int steps = (waitTime * 1000) / interval;

                    for (int i = 0; i <= steps; i++)
                    {
                        task.Value = (i / (float)steps * 100);
                        await Task.Delay(interval);
                    }
                });
        }

        // ========================================
        // Character API
        // ========================================
        public async Task CharacterInfoUIAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/characters/{characterName}";
            var requestBody = new {};

            HttpResponseMessage response = await _client.GetAsync(url);
            var characterInfoResponse = await HandleGetResponse<characterInfoResponse>(response);

            if(characterInfoResponse != null && characterInfoResponse is characterInfoResponse)
            {
                ui.CharacterInfoUI(characterInfoResponse);
                ui.InventoryInfoUI(characterInfoResponse);
            }
            else
            {
                Console.WriteLine("(ArtifactAPIService-L181) Failed to convert API Response to Character Data.");
            }

        }

        public async Task<characterInfoResponse?> CharacterInfoAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/characters/{characterName}";
            var requestBody = new {};

            HttpResponseMessage response = await _client.GetAsync(url);
            var characterInfoResponse = await HandleGetResponse<characterInfoResponse>(response);

            if(characterInfoResponse != null && characterInfoResponse is characterInfoResponse)
            {
                return characterInfoResponse;
            }
            else
            {
                Console.WriteLine("(ArtifactAPIService-L181) Failed to convert API Response to Character Data.");
                return null;
            }

        }
        
        // ========================================
        // API Send and Response Handling Methods 
        // ========================================
        public async Task<HttpResponseMessage> SendPostRequest(string url, object requestBody, string token)
        {
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Accept", "application/jason");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return await _client.PostAsync(url, content);
        }
    }
}