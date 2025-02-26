using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArtifactMMO
{
    public class ArtifactIntro
    {

        private static readonly HttpClient client = new HttpClient();


        public static async Task Main(string[] args)
        {
            int input = -1, x = 0, y = 0;
            string characterName = "SaltyNZ";
            string token = Environment.GetEnvironmentVariable("ArtifactAPIKey") ?? "NoToken";
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


                    if (input == 1) //Movement
                    {
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
                                await MoveCharacterAsync(characterName, token, x, y);
                                await Task.Delay(5000); //Standard movement delay
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


                    }
                    else if (input == 2) //Attack
                    {
                        await AttackAsync(characterName, token);
                    }
                    else if (input == 3) //Rest
                    {
                        await RestAsync(characterName, token);
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
