using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;

namespace ArtifactMMO
{
    public class ArtifactApiService
    {
        private readonly HttpClient _client;

        public ArtifactApiService()
        {
            _client = new HttpClient();
        }

        // ====================
        // Action API Methods 
        // ====================
        public async Task MoveCharacterAsync(string characterName, string token, int x, int y)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/move";
            var requestBody = new { x, y };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<MoveResponse>(response);
        }

        public async Task AttackAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/fight";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<AttackResponse>(response);
        }

        public async Task RestAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/rest";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<RestResponse>(response);
        }

        public async Task GatheringAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/gathering";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<gatheringResponse>(response);
        }

        public async Task UnequipAsync(string characterName, string token, string slot)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/unequip";
            var requestBody = new { slot };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<unequipResponse>(response);
        }

        public async Task EquipAsync(string characterName, string token, string code, string slot)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/equip";
            var requestBody = new { code, slot };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<equipResponse>(response);
        }

         public async Task CraftAsync(string characterName, string token, string code, int quantity)
         {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/crafting";
            var requestBody = new { code, quantity };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse<craftResponse>(response);
         }

        // ========================================
        // Character API
        // ========================================
        public async Task CharacterInfoAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/characters/{characterName}";
            var requestBody = new {};

            HttpResponseMessage response = await _client.GetAsync(url);
            await HandleResponse<characterInfoResponse>(response);
        }
        
        // ========================================
        // API Send and Response Handling Methods 
        // ========================================
        private async Task<HttpResponseMessage> SendPostRequest(string url, object requestBody, string token)
        {
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Accept", "application/jason");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return await _client.PostAsync(url, content);
        }

        private async Task HandleResponse<TResponse>(HttpResponseMessage response) where TResponse : class
        {
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<TResponse>>(result);

                string formattedResult = JsonSerializer.Serialize(JsonDocument.Parse(result), new JsonSerializerOptions
                {
                    WriteIndented = true // Adds line breaks & indentation
                });

                AnsiConsole.Write(
                    new Panel(Markup.Escape(formattedResult))
                        .Header("Response JSON")
                        .Expand()
                        .RoundedBorder()
                        .BorderColor(Spectre.Console.Color.Blue)
                );
                //Console.WriteLine("It did actually work"); //Testing Line

                if(apiResponse?.Data is IHasCooldown cooldownData && cooldownData.Cooldown != null)
                {
                    int waitTime = cooldownData.Cooldown is not null
                    ? (int)(cooldownData.Cooldown.Expiration - cooldownData.Cooldown.StartedAt).TotalSeconds : 0;
                    Console.WriteLine($"Waittime = {waitTime}");

                    if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
                }

            }
            else
            {
                Console.WriteLine($"Error-01: {response.StatusCode}");
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw Response: {result}");
            }
        }

        // ===============================
        // Progress Bar Methods
        // ===============================
        private static async Task ShowProgressBar(string taskName, int waitTime)
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
    }
}