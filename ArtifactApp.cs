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
                            await api.AttackAsync(characterName, token);
                            break;
                        case 3:
                            await api.RestAsync(characterName, token);
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
    }
}
