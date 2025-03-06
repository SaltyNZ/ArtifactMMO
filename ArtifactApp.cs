using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

    /*
    Current TODO List:
    - Add Loading Bar with the cooldown recived from API - Done - 27/2/25
    - Add more basic options to do basic game mechanics such as chop wood and craft. - Done - 28/2/25
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
            //Setting Variables
            int input = -1, x = 0, y = 0, qty = 1;
            string? characterName = "SaltyNZ", slot = "NoSlot", code = "NoCode";
            string token = Environment.GetEnvironmentVariable("ArtifactAPIKey") ?? "NoToken";

            Console.WriteLine("Please type character");
            characterName = Console.ReadLine();
            
            //Classes
            ArtifactApiService api = new ArtifactApiService();
            UI ui = new UI();
            AutomationScripts auto = new AutomationScripts();

            //Main Process
            while (input != 0)
            {
                ui.MainUIWriteLine();
                string? userInput = Console.ReadLine();
                if (int.TryParse(userInput, out input))
                {

                    switch (input)
                    {
                        case 1:
                            await api.CharacterInfoUIAsync(characterName ?? "", token);
                            break;
                        case 2:
                            Console.WriteLine("Select X");
                            if (int.TryParse(Console.ReadLine(), out x))
                            {
                                Console.WriteLine("Select Y");
                                if (int.TryParse(Console.ReadLine(), out y))
                                {
                                    await api.MoveCharacterAsync(characterName ?? "", token, x, y);
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
                        case 3:
                            await api.AttackAsync(characterName ?? "", token);
                            break;
                        case 4:
                            await api.RestAsync(characterName ?? "", token);
                            break;
                        case 5:
                            await api.GatheringAsync(characterName ?? "", token);
                            break;
                        case 6:
                            Console.WriteLine("Please type the slot");
                            slot = Console.ReadLine() ?? "";
                            if(ui.isValidEquipment(slot) && slot != null) await api.UnequipAsync(characterName ?? "", token, slot.ToLower());
                            break;
                        case 7:
                            Console.WriteLine("Please type the slot");
                            slot = Console.ReadLine() ?? "";
                            if(ui.isValidEquipment(slot) && slot != null)
                            {
                                Console.WriteLine("Please type the item code to equip");
                                code = Console.ReadLine() ?? "";
                                if(ui.isValidCraft(code) && code != null) await api.EquipAsync(characterName ?? "",token,code.ToLower(),slot.ToLower());
                            }
                            break;
                        case 8:
                            Console.WriteLine("Please type what you want to craft");
                            code = Console.ReadLine() ?? "";
                            Console.WriteLine("Please select No of items");
                            if(int.TryParse(Console.ReadLine(), out qty))
                            {
                                if(ui.isValidCraft(userInput ?? "") && userInput != null) await api.CraftAsync(characterName ?? "",token,code.ToLower(),qty);
                            }
                            else
                            {
                                Console.WriteLine("Invalid QTY");
                            }                            
                            break;
                        case 9:
                            await auto.AutoIngotGathering(characterName ?? "", token);
                            break;
                        case 10:
                            await auto.AutoAttack(characterName ?? "", token);
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