using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;

    /*
        2.0 Rewrite
            - Redo the UI fully using Console.Spectre as the foundation from the get go to provide clarity.
            - Use SQLLite where possible.
            - Redo the API Calling process to be more streamlined and less garbled.
            - Start work on the Fully automated gathering system using SQLLite
            - Create a Ordering system for the crafter.
    */

namespace ArtifactMMO
{
    public class ArtifactApp
    {
        public static async Task Main(string[] args)
        {
            // ----------------------
            //    Define Variables
            // ----------------------
            
            //Variables
            string token = Environment.GetEnvironmentVariable("ArtifactAPIKey") ?? "NoToken";

            //Classes
            ArtifactApiService api = new ArtifactApiService();
            UI ui = new UI();
            AutomationScripts auto = new AutomationScripts();


            // -------------------
            //    Start up Menu
            // -------------------
            var characterName = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your [red]character's[/] name?"));
            AnsiConsole.WriteLine($"Character name is: {characterName}");

            var uiChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select what command to use: :computer_disk:[/]")
                    .PageSize(10)
                    .MoreChoicesText("[blue](Move up and down to see more selections)[/]")
                    .AddChoices(new[] {
                        "Character Info","Auto Ingot",
                        "Auto Attack","Auto Plank",
                        "Auto Task","Exit"
                }));

            switch (uiChoice)
            {
                case "Character Info":
                    await api.CharacterInfoUIAsync(characterName ?? "", token);
                    break;
                
                case "Auto Ingot":
                    AnsiConsole.WriteLine($"You selected {uiChoice}");
                    break;

                case "Auto Attack":
                    AnsiConsole.WriteLine($"You selected {uiChoice}");
                    break;

                case "Auto Plank":
                    AnsiConsole.WriteLine($"You selected {uiChoice}");
                    break;
                
                case "Auto Task":
                    AnsiConsole.WriteLine($"You selected {uiChoice}");
                    break;

                case "Exit":
                    AnsiConsole.WriteLine($"You selected {uiChoice}");
                    break;
            }   
        }
    }
}