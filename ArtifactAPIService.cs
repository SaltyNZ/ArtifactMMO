using System;
using System.Net.Http;
using System.Reflection.Metadata;
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
        UI ui = new UI();
        bool debug = false;

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
            MoveResponse? apiResponse = await HandlePostResponse<MoveResponse>(response);

            if(apiResponse != null && apiResponse is MoveResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }
        }

        public async Task AttackAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/fight";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            AttackResponse? apiResponse = await HandlePostResponse<AttackResponse>(response);

            if(apiResponse != null && apiResponse is AttackResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }
        }

        public async Task RestAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/rest";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            RestResponse? apiResponse = await HandlePostResponse<RestResponse>(response);

            if(apiResponse != null && apiResponse is RestResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }
        }

        public async Task GatheringAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/gathering";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            gatheringResponse? apiResponse = await HandlePostResponse<gatheringResponse>(response);

            if(apiResponse != null && apiResponse is gatheringResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }

        }

        public async Task UnequipAsync(string characterName, string token, string slot)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/unequip";
            var requestBody = new { slot };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            unequipResponse? apiResponse = await HandlePostResponse<unequipResponse>(response);

            if(apiResponse != null && apiResponse is unequipResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }
        }

        public async Task EquipAsync(string characterName, string token, string code, string slot)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/equip";
            var requestBody = new { code, slot };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            equipResponse? apiResponse = await HandlePostResponse<equipResponse>(response);

            if(apiResponse != null && apiResponse is equipResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }
        }

        public async Task<craftResponse> CraftAsync(string characterName, string token, string code, int quantity)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/crafting";
            var requestBody = new { code, quantity };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            craftResponse? apiResponse = await HandlePostResponse<craftResponse>(response);

            if(apiResponse != null && apiResponse is craftResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }

            return apiResponse;

        }

        public async Task BankItemsAsync(string characterName, string token, string? code, int quantity)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/bank/deposit";
            var requestBody = new { code, quantity };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            bankItemResponse? apiResponse = await HandlePostResponse<bankItemResponse>(response);

            if(apiResponse != null && apiResponse is bankItemResponse)
            {
                int waitTime = apiResponse.Cooldown is not null
                ? (int)(apiResponse.Cooldown.Expiration - apiResponse.Cooldown.StartedAt).TotalSeconds : 0;
                Console.WriteLine($"Waittime = {waitTime}");

                if (waitTime > 0) await ShowProgressBar("Cooldown in progress...", waitTime);
            }
        }

        // ========================================
        // Character API
        // ========================================
        public async Task CharacterInfoAsync(string characterName, string token)
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
    }
}