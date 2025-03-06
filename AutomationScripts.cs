using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;


namespace ArtifactMMO
{
    public class AutomationScripts
    {
        ArtifactApiService api = new ArtifactApiService();
        private readonly HttpClient _client;
        public AutomationScripts()
        {
            _client = new HttpClient();
        }

        public async Task AutoIngotGathering(string characterName, string token)
        {
            /*
                Steps:
                1. Start up:
                    - What Ingot - Done
                    - Get Char info - Done
                    - Clear Inventory into bank if needed - Done
                    - return to mine - Done
                2. The Loop
                    A. Gather until the inventory is at capacity - Done
                    B. Move to crafting station and craft. - Done
                    C. Bank the Ingots - Done
                    D. Move to gathering and repeat - Done

            */
            
            int totalItems = 0, totalOre = 0;

            Console.WriteLine("type the code for the ingot:");
            string? ingot = Console.ReadLine().ToLower() ?? null;
            if(ingot == null)
            {
                Console.WriteLine("Invalid Item Code");
                return;
            }
            string ore = $"{ingot}_ore";
            
            
            bool breaked = false;


            Console.WriteLine("STARTING GATHERING TASK");
            string url = $"https://api.artifactsmmo.com/characters/{characterName}";
            var requestBody = new {};

            HttpResponseMessage response = await _client.GetAsync(url);
            var charinfo = await api.HandleGetResponse<characterInfoResponse>(response);


            //get location that will be the primary resource gathering.
            if(charinfo != null)
            {
                int gatherX = charinfo.X, gatherY = charinfo.Y;

                Console.WriteLine("Emptying Inventory");
                if(charinfo.Inventory != null)
                {
                    await api.MoveCharacterAsync(characterName, token, 4, 1);
                    foreach(var item in charinfo.Inventory)
                    {
                        if(item.Quantity > 0 && item.Code != ore)
                        {
                            await api.BankItemsAsync(characterName, token, item.Code, item.Quantity);
                        } 
                        else if (item.Quantity > 0 && item.Code == ore)
                        {
                            totalItems += item.Quantity;
                            totalOre += item.Quantity;
                            Console.WriteLine($"There is {item.Quantity} in the inv so the total is now {totalItems} for total items");
                        }
                    }
                    
                }

                await api.MoveCharacterAsync(characterName,token,gatherX,gatherY);

                //loop
                while(true)
                {                       
                    while(totalItems < charinfo.InventoryMaxItems) // or statement for testing
                    {
                        if(Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(intercept: true).Key;

                            if(key == ConsoleKey.Escape)
                            {
                                Console.WriteLine("Ending Loop as ESC was pressed");
                                breaked = true;
                                break;
                            }
                        }

                        url = $"https://api.artifactsmmo.com/my/{characterName}/action/gathering";
                        requestBody = new { };

                        response = await api.SendPostRequest(url, requestBody, token);
                        gatheringResponse? apiGatherResponse = await api.HandlePostResponse<gatheringResponse>(response);

                        if(apiGatherResponse != null && apiGatherResponse is gatheringResponse)
                        {
                            int waitTime = apiGatherResponse.Cooldown is not null
                            ? (int)(apiGatherResponse.Cooldown.Expiration - apiGatherResponse.Cooldown.StartedAt).TotalSeconds : 0;                           

                            if (waitTime > 0)
                            {
                                await ArtifactApiService.ShowProgressBar("Gathering in progress...", waitTime);
                            }
                            else
                            {
                                Console.WriteLine($"weird waittime");
                                break;
                            }
                            foreach(var gathereditem in apiGatherResponse.Detail.Items)
                            {
                                if(gathereditem.Code == ore)
                                {
                                    totalItems += gathereditem.Quantity;
                                    totalOre += gathereditem.Quantity;
                                    Console.WriteLine($"Gathered {gathereditem.Quantity} of {gathereditem.Code} so the total is {totalItems}");
                                }
                                else if (gathereditem.Quantity > 0)
                                {
                                    totalItems += gathereditem.Quantity;
                                    Console.WriteLine($"Gathered {gathereditem.Quantity} of {gathereditem.Code} so the total is {totalItems}");
                                }
                            }
                        }
                    }

                    if(breaked == true) break;


                    // -----------------
                    //     CRAFTING
                    // -----------------
                    Console.WriteLine("Moving to Crafting Station");
                    await api.MoveCharacterAsync(characterName, token, 1, 5);

                    //Crafting Variables
                    int qty = totalOre / 10;
                    Console.WriteLine($"{qty}");
                    string code = ingot.ToLower();
                    Console.WriteLine($"{code}");
                
                    //API
                    if(qty > 0)
                    {
                        Console.WriteLine($"Crafting {qty} {code} ingots");
                        craftResponse? apiCraftResponse = await api.CraftAsync(characterName,token,code.ToLower(),qty);                      

                        // -----------------
                        //  BANKING INGOTS
                        // -----------------
                        Console.WriteLine("Moving to the bank");
                        await api.MoveCharacterAsync(characterName, token, 4, 1);
                        Console.WriteLine("Moving Items to bank");
                        totalItems = 0;
                        totalOre = 0;
                        foreach(var craftItem in apiCraftResponse.Character.Inventory)
                        {
                            if(craftItem.Quantity > 0 && craftItem.Code != ore)
                            {
                                await api.BankItemsAsync(characterName, token, craftItem.Code, craftItem.Quantity);
                            } 
                            else if (craftItem.Quantity > 0 && craftItem.Code == ore)
                            {
                                totalItems = craftItem.Quantity;
                                totalOre = craftItem.Quantity;
                                Console.WriteLine($"There is still {craftItem.Quantity} {ore} in the inv after crafting so the total is now {totalItems} for total items");
                            }
                        }
                    }
                    
                    Console.WriteLine($"Moving back to {ingot} and restart to the top");
                    await api.MoveCharacterAsync(characterName, token, gatherX, gatherY);


                    if(Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;

                        if(key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("Ending Loop as ESC was pressed");
                            break;
                        }
                    }
                    Console.WriteLine("Loop End");          
                }
            }
        }
        
        public async Task AutoAttack(string characterName, string token)
        {
            // -----------------------------------------------------------------------------
            // TODO
            //  - Add Start Up Sequence
            //  - Add Attacking
            //  - Add Conditional Resting
            //  - Add Bank Depositing if inventory is full
            //  - Make sure the variables are rest correctly at the start of each loop
            // -----------------------------------------------------------------------------

            characterInfoResponse charinfo = await api.CharacterInfoAsync(characterName, token);

            if(charinfo != null && charinfo is characterInfoResponse)
            {
                //DEFINE VARIABLES AND CLASSES
                //Vars
                int attackX = charinfo.X, attackY = charinfo.Y;
                //Classes
                AttackResponse attackInfo = new AttackResponse();
                bankItemResponse bankItemInfo = new bankItemResponse();



                //PRE-LOOP CHECKS

                //if not full HP rest
                if(charinfo.Hp != charinfo.MaxHp) {await api.RestAsync(characterName, token);}

                while (true)
                {
                    
                }
            }
            
        }
    }
}