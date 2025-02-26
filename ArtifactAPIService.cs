using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArtifactMMO
{
    public class ArtifactApiService
    {
        private readonly HttpClient _client;

        public ArtifactApiService()
        {
            _client = new HttpClient();
        }

        public async Task MoveCharacterAsync(string characterName, string token, int x, int y)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/move";
            var requestBody = new { x, y };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse(response);
        }

        public async Task AttackAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/fight";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse(response);
        }

        public async Task RestAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/rest";
            var requestBody = new { };

            HttpResponseMessage response = await SendPostRequest(url, requestBody, token);
            await HandleResponse(response);
        }

        private async Task<HttpResponseMessage> SendPostRequest(string url, object requestBody, string token)
        {
            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Accept", "application/jason");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return await _client.PostAsync(url, content);
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                Console.WriteLine("It did actually work");
            }
            else
            {
                Console.WriteLine($"Error-01: {response.StatusCode}");
            }
        }
    }
}