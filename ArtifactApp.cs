﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;
using Microsoft.Data.Sqlite;

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
            SQLiteScript sql = new SQLiteScript();


            // -------------------
            //    Start up Menu
            // -------------------
            var characterName = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your [red]character's[/] name?"));
            AnsiConsole.WriteLine($"Character name is: {characterName}");
            string choice = "No";
            while(choice != "Exit")
            {
                var uiChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Select what command to use: :computer_disk:[/]")
                        .PageSize(10)
                        .MoreChoicesText("[blue](Move up and down to see more selections)[/]")
                        .AddChoices(new[] {
                            "Character Info","Auto Ingot", "Auto Gathering",
                            "Auto Attack","Auto Plank","Auto Mining Level",
                            "Generate Database","Exit"
                            //,"ManualQ"  //Comment Out for Working Build.
                    }));

                switch (uiChoice)
                {
                    case "Character Info":
                        await api.CharacterInfoUIAsync(characterName ?? "", token);
                        break;
                    
                    case "Auto Ingot":
                        await auto.AutoIngotGathering(characterName ?? "", token);
                        AnsiConsole.WriteLine($"You selected {uiChoice}");
                        break;

                    case "Auto Attack":
                        await auto.AutoAttack(characterName ?? "", token);
                        AnsiConsole.WriteLine($"You selected {uiChoice}");
                        break;

                    case "Auto Plank":
                        await auto.AutoPlankGathering(characterName ?? "", token);
                        AnsiConsole.WriteLine($"You selected {uiChoice}");
                        break;
                    
                    case "Auto Gathering":
                        await auto.AutoBaseGathering(characterName ?? "", token);
                        AnsiConsole.WriteLine($"You selected {uiChoice}");
                        break;
                    
                    case "Auto Mining Level":
                        await auto.AutoMiningLeveling(characterName ?? "", token);
                        break;

                    case "Exit":
                        AnsiConsole.WriteLine($"You selected {uiChoice}");
                        choice = uiChoice;
                        break;
                    
                    case "Generate Database":
                        await sql.SQLiteUpdate();
                        break;

                    case "ManualQ":
                        //await api.PerformActionAsync<AttackResponse>(characterName, token, "fight", new{}, "Attacking Mob:");
                        await sql.SQLiteUpdate();
                        break;
                }   
            }    
        }
    }
}