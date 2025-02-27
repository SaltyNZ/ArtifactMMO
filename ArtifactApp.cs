﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

    /*
    Current TODO List:
    - Add Loading Bar with the cooldown recived from API - Done - 27/2/25
    - Add more basic options to do basic game mechanics such as chop wood and craft.
    - Add some Automation using the basic options
    - Get a report of data so you dont need to see the screen like Inventory, Level, Location, gold etc.
    - Optimize and Improve the Console UI using Spectre.Console
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
            UI ui = new UI();
            while (input != 0)
            {
                ui.MainUIWriteLine();
                string? userInput = Console.ReadLine();
                if (int.TryParse(userInput, out input))
                {

                    switch (input)
                    {
                        case 1:
                            Console.WriteLine("Select X");
                            if (int.TryParse(Console.ReadLine(), out x))
                            {
                                Console.WriteLine("Select Y");
                                if (int.TryParse(Console.ReadLine(), out y))
                                {
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
                        case 4:
                            await api.GatheringAsync(characterName, token);
                            break;
                        case 5:
                            Console.WriteLine("Please type the slot");
                            userInput = Console.ReadLine();
                            if(ui.isValidEquipment(userInput ?? "") && userInput != null) await api.UnequipAsync(characterName, token, userInput.ToLower());
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
