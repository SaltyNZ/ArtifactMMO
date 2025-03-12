using System;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
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

            gatheringResponse? apiGatherResponse = new gatheringResponse();
            craftResponse? apiCraftResponse = new craftResponse();

            Console.WriteLine("type the code for the ingot:");
            string? ingot = Console.ReadLine()?.ToLower();
            if(ingot == null)
            {
                Console.WriteLine("Invalid Item Code");
                return;
            }
            string ore = $"{ingot}_ore";


            Console.WriteLine("STARTING GATHERING TASK");

            var charinfo = await api.CharacterInfoAsync(characterName, token);


            //get location that will be the primary resource gathering.
            if(charinfo != null)
            {
                int gatherX = charinfo.X, gatherY = charinfo.Y;

                Console.WriteLine("Emptying Inventory");
                if(charinfo.Inventory != null)
                {
                    //await api.MoveCharacterAsync(characterName, token, 4, 1);
                    await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=4, y=1}, "Moving:");
                    foreach(var item in charinfo.Inventory)
                    {
                        if(item.Quantity > 0 && item.Code != ore)
                        {
                            //await api.BankItemsAsync(characterName, token, item.Code, item.Quantity);
                            await api.PerformActionAsync<bankItemResponse>(characterName, token, "bank/deposit", new {code = item.Code, quantity = item.Quantity}, $"Banking {item.Code}:");
                        } 
                        else if (item.Quantity > 0 && item.Code == ore)
                        {
                            totalItems += item.Quantity;
                            totalOre += item.Quantity;
                            Console.WriteLine($"There is {item.Quantity} in the inv so the total is now {totalItems} for total items");
                        }
                    }
                    
                }

                //await api.MoveCharacterAsync(characterName,token,gatherX,gatherY);
                await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=gatherX, y=gatherY}, "Moving:");

                //loop
                while(true)
                {                       
                    while(totalItems < charinfo.InventoryMaxItems) // or statement for testing
                    {
                        apiGatherResponse = await api.PerformActionAsync<gatheringResponse>(characterName, token, "gathering", new{}, "Gathering:");
                        
                        if(apiGatherResponse != null && apiGatherResponse is gatheringResponse)
                        {
                            if (apiGatherResponse?.Detail?.Items != null)
                            {
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
                    
                    }


                    // -----------------
                    //     CRAFTING
                    // -----------------
                    Console.WriteLine("Moving to Crafting Station");
                    await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=1, y=5}, "Moving to crafting:");

                    //Crafting Variables
                    int qty = totalOre / 10;
                    Console.WriteLine($"{qty}");
                    string code = ingot.ToLower();
                    Console.WriteLine($"{code}");
                
                    //API
                    if(qty > 0)
                    {
                        Console.WriteLine($"Crafting {qty} {code} ingots");
                        //apiCraftResponse = await api.CraftAsync(characterName,token,code.ToLower(),qty);
                        apiCraftResponse = await api.PerformActionAsync<craftResponse>(characterName, token, "crafting", new { code = code, quantity = qty}, $"Crafting {code}");                      

                        // -----------------
                        //  BANKING INGOTS
                        // -----------------

                        await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=4, y=1}, "Moving to bank:");

                        totalItems = 0;
                        totalOre = 0;

                        if(apiCraftResponse?.Character?.Inventory != null)
                        {
                            foreach(var craftItem in apiCraftResponse.Character.Inventory)
                            {
                                if(craftItem.Quantity > 0 && craftItem.Code != ore)
                                {
                                    await api.PerformActionAsync<bankItemResponse>(characterName, token, "bank/deposit", new {code = craftItem.Code, quantity = craftItem.Quantity}, "Banking:");
                                } 
                                else if (craftItem.Quantity > 0 && craftItem.Code == ore)
                                {
                                    totalItems = craftItem.Quantity;
                                    totalOre = craftItem.Quantity;
                                    Console.WriteLine($"There is still {craftItem.Quantity} {ore} in the inv after crafting so the total is now {totalItems} for total items");
                                }
                            }
                        }
                        
                    }
                    
                    Console.WriteLine($"Moving back to {ingot} and restart to the top");
                    await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=gatherX, y=gatherY}, "Moving:");

                    Console.WriteLine("Loop End");          
                }
            }
        }
        
        public async Task AutoAttack(string characterName, string token)
        {
            // -----------------------------------------------------------------------------
            //  TODO - Done 7/03/2025
            //  - Add Start Up Sequence
            //  - Add Attacking
            //  - Add Conditional Resting
            //  - Add Bank Depositing if inventory is full
            //  - Make sure the variables are rest correctly at the start of each loop
            // -----------------------------------------------------------------------------

            characterInfoResponse? charinfo = await api.CharacterInfoAsync(characterName, token);

            if(charinfo != null && charinfo is characterInfoResponse)
            {
                //DEFINE VARIABLES AND CLASSES
                //Vars
                int attackX = charinfo.X, attackY = charinfo.Y, totalItems = 0, maxItems = 0;
                double healthLeft, healthLeftPercent = 0;
                Console.WriteLine("Type health left as percent decimal example (0.4)");

                string? healthLeftstring = Console.ReadLine();
                if(double.TryParse(healthLeftstring, out double healthLeftout))
                {
                    healthLeftPercent = healthLeftout;
                }
                //Classes
                AttackResponse? attackInfo = new AttackResponse();
                bankItemResponse? bankItemInfo = new bankItemResponse();
                MoveResponse? moveInfo = new MoveResponse();



                //PRE-LOOP CHECKS

                //if not full HP rest
                if(charinfo.Hp != charinfo.MaxHp) {await api.RestAsync(characterName, token);}

                while (true)
                {
                    moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move",new {x=4, y=1}, "Moving to the Bank");

                    if(moveInfo != null)
                    {
                        if(moveInfo?.Character?.Inventory != null)
                        {
                            foreach(var item in moveInfo.Character.Inventory)
                            {
                                if(item.Quantity > 0)
                                {
                                    await api.PerformActionAsync<bankItemResponse>(characterName, token, "bank/deposit", new {code = item.Code, quantity = item.Quantity}, $"Banking {item.Code}:");
                                }
                            }

                            totalItems = 0;
                            maxItems = moveInfo.Character.InventoryMaxItems;
                            moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move",new {x=attackX, y=attackY}, "Moving to the Mob");

                        }
                        
                    }

                    while(totalItems < maxItems)
                    {
                        attackInfo = await api.PerformActionAsync<AttackResponse>(characterName, token, "attack", new{}, "Attacking Mob");

                        if(attackInfo?.Character != null)
                        {
                            healthLeft = (double)attackInfo.Character.Hp/(double)attackInfo.Character.MaxHp;
                            if(healthLeft < healthLeftPercent)
                            {
                                Console.WriteLine($"Resting because the hp is at {healthLeft*100}% of Max HP");
                                await api.RestAsync(characterName, token);
                            }
                        }
                        if(attackInfo?.Fight?.Drops != null)
                        {
                            foreach(var drop in attackInfo.Fight.Drops)
                            {
                                if(drop.Quantity > 0)
                                {
                                    totalItems += drop.Quantity;
                                    Console.WriteLine($"The total amount of items is {totalItems}");
                                }
                            }
                        }                       
                    }

                    
                }
            }
            
        }
    
        public async Task AutoBasicItemTask(string characterName, string token)
        {
            //----------------------------------------------------
            //  TODO
            //  - Create Starting Sequence
            //  - Harvest Item
            //  - Trade in item
            //  - Complete Task when all items are handed in
            //  - Stop when Task completed
            //----------------------------------------------------

            characterInfoResponse? charinfo = await api.CharacterInfoAsync(characterName, token);

            if(charinfo != null)
            {
                //DEFINE VARIABLES AND CLASSES
                //Vars
                int gatherX = charinfo.X, gatherY = charinfo.Y, totalItems = 0, totalTaskItems = 0;
                //Classes
                gatheringResponse gatheringInfo = new gatheringResponse();
                MoveResponse? moveInfo = new MoveResponse();

                Console.WriteLine(totalItems);
                Console.WriteLine(totalTaskItems);
            }
            
    }

        public async Task AutoPlankGathering(string characterName, string token)
        {
            characterInfoResponse? charInfo = await api.CharacterInfoAsync(characterName, token);

            if(charInfo != null)
            {
                int? totalItems = 0, maxItems = charInfo.InventoryMaxItems;
                int  totalWood = 0, qty= 0, gatherX = charInfo.X, gatherY = charInfo.Y;
                gatheringResponse? gatheringInfo = new gatheringResponse();
                bankItemResponse? bankItemInfo = new bankItemResponse();
                MoveResponse? moveInfo = new MoveResponse();
                craftResponse? craftInfo = new craftResponse();


                Console.WriteLine("Please type a wood type:");
                string? wood = Console.ReadLine()?.ToLower();
                string plank = $"{wood}_plank";
                wood = $"{wood}_wood";


                
                
                Console.WriteLine($"Wood Code = {wood}");
                Console.WriteLine($"Plank Code = {plank}");

                //get current items total
                if(charInfo?.Inventory != null)
                {
                    foreach(var item in charInfo.Inventory)
                    {
                        if(item.Code == wood)
                        {
                            totalWood += item.Quantity;
                            totalItems += item.Quantity;
                            Console.WriteLine($"There was {totalWood} of {wood} in the inventory and the overall item total is {totalItems}.");
                            
                        } else if (item.Quantity > 0)
                        {
                            totalItems += item.Quantity;
                            Console.WriteLine($"There was {item.Quantity} of {item.Code} so the new total is {totalItems}");
                        }
                    }
                }

                //Start Loop
                while(true)
                {
                    while(totalItems < maxItems)
                    {
                        gatheringInfo = await api.GatheringAsync(characterName, token);

                        if(gatheringInfo?.Detail?.Items != null)
                        {
                            foreach(var item in gatheringInfo.Detail.Items)
                            {
                                if(item.Code == wood)
                                {
                                    totalWood += item.Quantity;
                                    totalItems += item.Quantity;
                                    Console.WriteLine($"gathered {item.Quantity} of {wood} the overall {wood} total is {totalWood} and INV total is {totalItems}.");
                                } else if (item.Quantity > 0)
                                {
                                    totalItems += item.Quantity;
                                    Console.WriteLine($"gathered {item.Quantity} of {item.Code} the INV total is {totalItems}.");
                                }
                            }
                        }
                        
                    }

                    //Crafting
                    await api.MoveCharacterAsync(characterName, token, -2, -3);

                    qty = totalWood/10;

                    if(qty > 0)
                    {
                        await api.CraftAsync(characterName, token, plank, qty);
                    }

                    Console.WriteLine("Moving to the bank");
                    moveInfo = await api.MoveCharacterAsync(characterName, token, 4, 1);
                    Console.WriteLine("Moving Items to bank");
                    totalItems = 0;
                    totalWood = 0;

                    if(moveInfo?.Character?.Inventory != null)
                    {
                        foreach(var item in moveInfo.Character.Inventory)
                        {
                            if(item.Quantity > 0 && item.Code != wood)
                            {
                                await api.BankItemsAsync(characterName, token, item.Code, item.Quantity);
                            } 
                            else if (item.Quantity > 0 && item.Code == wood)
                            {
                                totalItems = item.Quantity;
                                totalWood = item.Quantity;
                                Console.WriteLine($"There is still {item.Quantity} {wood} in the inv after crafting so the total is now {totalItems} for total items");
                            }
                        }
                    }

                    await api.MoveCharacterAsync(characterName, token, gatherX, gatherY);

                }
            }
        }
    }
}