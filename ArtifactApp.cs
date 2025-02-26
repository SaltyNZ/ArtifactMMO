using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

    /*
    Current TODO List:
    - Add more basic options to do basic game mechanics such as chop wood and craft.
    - Add some Automation using the basic options
    - Get a report of data so you dont need to see the screen like Inventory, Level, Location, gold etc.
    */

namespace ArtifactMMO
{
    public class ArtifactApp
    {

        private static readonly HttpClient client = new HttpClient();

        public static async Task Main(string[] args)
        {
            int input = -1, x = 0, y = 0;
            string characterName = "SaltyNZ";
            string token = Environment.GetEnvironmentVariable("ArtifactAPIKey") ?? "NoToken";

            ArtifactApiService api = new ArtifactApiService();

            while (input != 0)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Select a Function:");
                Console.WriteLine("1. Move");
                Console.WriteLine("2. Attack Current Square");
                Console.WriteLine("3. Rest");
                Console.WriteLine("0. Exit");
                string? userInput = Console.ReadLine();
                if (int.TryParse(userInput, out input))
                {

                    switch (input)
                    {
                        case 1:
                            Console.WriteLine("Select X");
                            userInput = Console.ReadLine();
                            if (int.TryParse(userInput, out int moveInput))
                            {
                                x = moveInput;
                                Console.WriteLine("Select Y");
                                userInput = Console.ReadLine();
                                if (int.TryParse(userInput, out moveInput))
                                {
                                    y = moveInput;
                                    await api.MoveCharacterAsync(characterName, token, x, y);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid number.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid number.");
                            }
                            break;
                        case 2:
                            await AttackAsync(characterName, token);
                            break;
                        case 3:
                            await RestAsync(characterName, token);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }

            Console.WriteLine("Exiting App");
        }


        public static async Task MoveCharacterAsync(string characterName, string token, int x, int y)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/move";

            var requestBody = new
            {
                x = x,
                y = y
            };

            string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }

        public static async Task AttackAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/fight";

            var requestBody = new
            {

            };

            string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

        }

        public static async Task RestAsync(string characterName, string token)
        {
            string url = $"https://api.artifactsmmo.com/my/{characterName}/action/rest";

            var requestBody = new
            {

            };

            string json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

        }

    }
}
