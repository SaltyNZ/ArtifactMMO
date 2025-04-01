using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;


namespace ArtifactMMO
{
    public class AutomationScripts
    {
        ArtifactApiService api = new ArtifactApiService();
        SQLiteScript sql = new SQLiteScript();
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
                if(charinfo.Hp != charinfo.MaxHp) {await api.PerformActionAsync<RestResponse>(characterName, token, "rest", new{}, "Resting:");}

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
                        attackInfo = await api.PerformActionAsync<AttackResponse>(characterName, token, "fight", new{}, "Attacking Mob:");

                        if(attackInfo?.Fight?.Drops != null)
                        {
                            foreach(var drop in attackInfo.Fight.Drops)
                            {
                                if(drop.Quantity > 0)
                                {
                                    totalItems += drop.Quantity;
                                    Console.WriteLine($"There is {drop.Quantity} {drop.Code}");
                                    Console.WriteLine($"The total amount of items is now {totalItems}");
                                }
                            }
                        }
                        if(attackInfo?.Character != null)
                        {
                            healthLeft = (double)attackInfo.Character.Hp/(double)attackInfo.Character.MaxHp;
                            if(healthLeft < healthLeftPercent)
                            {
                                Console.WriteLine($"Resting because the hp is at {healthLeft*100}% of Max HP");
                                await api.PerformActionAsync<RestResponse>(characterName, token, "rest", new{}, "Resting:");
                            }
                        }
                                             
                    }

                    
                }
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
                        gatheringInfo = await api.PerformActionAsync<gatheringResponse>(characterName, token, "gathering", new{}, $"Gathering {wood}:");

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
                    await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new {x = -2, y = -3}, "Moving to Crafting");

                    qty = totalWood/10;

                    if(qty > 0)
                    {
                        await api.PerformActionAsync<craftResponse>(characterName, token, "crafting", new {code = plank, quantity = qty}, $"Crafting {qty} - {plank}");
                    }

                    Console.WriteLine("Moving to the bank");
                    moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move",new {x=4, y=1}, "Moving to the Bank");
                    Console.WriteLine("Depositing Items into the bank");
                    totalItems = 0;
                    totalWood = 0;

                    if(moveInfo?.Character?.Inventory != null)
                    {
                        foreach(var item in moveInfo.Character.Inventory)
                        {
                            if(item.Quantity > 0 && item.Code != wood)
                            {
                                await api.PerformActionAsync<bankItemResponse>(characterName, token, "bank/deposit", new {code = item.Code, quantity = item.Quantity}, $"Banking {item.Code}:");
                            } 
                            else if (item.Quantity > 0 && item.Code == wood)
                            {
                                totalItems = item.Quantity;
                                totalWood = item.Quantity;
                                Console.WriteLine($"There is still {item.Quantity} {wood} in the inv after crafting so the total is now {totalItems} for total items");
                            }
                        }
                    }

                    await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new {x = gatherX, y = gatherY}, $"Moving to {wood}");

                }
            }
        }
    
        public async Task AutoBaseGathering(string characterName, string token)
        {
            characterInfoResponse? charInfo = await api.CharacterInfoAsync(characterName, token);

            if(charInfo != null)
            {
                int? totalItems = 0, maxItems = charInfo.InventoryMaxItems;
                int  gatherX = charInfo.X, gatherY = charInfo.Y;
                gatheringResponse? gatheringInfo = new gatheringResponse();
                bankItemResponse? bankItemInfo = new bankItemResponse();
                MoveResponse? moveInfo = new MoveResponse();

                if(charInfo?.Inventory != null)
                {
                    foreach(var item in charInfo.Inventory)
                    {
                        if (item.Quantity > 0)
                        {
                            totalItems += item.Quantity;
                            Console.WriteLine($"There was {item.Quantity} of {item.Code} so the new total is {totalItems}");
                        }
                    }
                }

                while(true)
                {
                    while(totalItems < maxItems)
                    {
                        gatheringInfo = await api.PerformActionAsync<gatheringResponse>(characterName, token, "gathering", new{}, $"Gathering item:");

                        if(gatheringInfo?.Detail?.Items != null)
                        {
                            foreach(var item in gatheringInfo.Detail.Items)
                            {
                                if (item.Quantity > 0)
                                {
                                    totalItems += item.Quantity;
                                    Console.WriteLine($"There was {item.Quantity} of {item.Code} so the new total is {totalItems}");
                                }
                            }
                        }
                    }

                    moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move",new {x=4, y=1}, "Moving to the Bank");

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
                    }

                    await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new {x = gatherX, y = gatherY}, $"Moving to Gathering Spot");
                }
            }
        }
    
        public async Task AutoMiningLeveling(string characterName, string token)
        {
            /*
            
            -----------------------------------THE PLAN-----------------------------------
            Create a script that automatically levels up a player in mining skill.
            Use SQLite and other skills learnt to automate the process.

            Rough outline:
                - Set global variables
                - Start While True loop
                    - Get relevant infomation through the use of the SQLite database
                        Resources required, The level cap, the item to craft ETC
                    - Set the relevant variables that chagne per item.
                        Like resources needed to craft ETC (Steel is Iron and Coal)
                    - Empty inventory leaving the needed resources
                    - Start Level Checking loop
                        - Move to the resource and gather (Need checking to handle 2 item crafts)
                        - Move to next resource if required.
                        - Craft item
                        - Deposit
                        - Set level to check if it needs to move on.
                        - Loop or Break
                    -Loop
            
            NOTES:
            //MANUAL ENTRY HERE - refers to data that will need to be changed to work with other skills and needs to be looked at if I want to create a 1 script fits all

            */

            //Call Character
            characterInfoResponse? charInfo = await api.CharacterInfoAsync(characterName, token);

            //Set variables
            MoveResponse? moveInfo = new MoveResponse();
            gatheringResponse? gatherInfo = new gatheringResponse();
            craftResponse? craftInfo = new craftResponse();
            if(charInfo != null)
            {
                int? level = charInfo.MiningLevel, //MANUAL ENTRY HERE
                leveltarget = 999, currentBestLevel = 0, maxitems = charInfo.InventoryMaxItems, craftable = 0, gathereditemMax = 0, currentTotal = 0, gathereditems = 0, gatheredcraftable = 0, itemcraftqty = 0;
                string? currentBestItem = "";
                

                while(true)
                {
                    leveltarget = 999; 
                    currentBestLevel = 0; 
                    craftable = 0; 
                    gathereditemMax = 0; 
                    currentTotal = 0; 
                    gathereditems = 0;
                    gatheredcraftable = 0;
                    itemcraftqty = 0;
                    //Relevant Infomation form SQLite
                    var resourceHDRData = await sql.RetrieveResourceHdrData();
                    var resourceLineData = await sql.RetrieveResourceLineData();
                    var itemHDRData = await sql.RetrieveItemHdrData("mining", "resource", "'bar','alloy'"); //MANUAL ENTRY HERE
                    
                    //Set up logic

                    //Get the item to craft and the next item trigger to break loop
                    foreach(var hdr in itemHDRData)
                    {
                        
                        if(hdr.Level <= level && (hdr.Code != "obsidian" || hdr.Code != "strangold")) //MANUAL ENTRY HERE
                        {
                            currentBestLevel = hdr.Level;
                            currentBestItem = hdr.Code;
                        }
                        else if (hdr.Level > level && hdr.Level < leveltarget)
                        {
                            leveltarget = hdr.Level;
                        }
                        
                    }
                    
                    Console.WriteLine($"The Character Skill Level is {level}");
                    Console.WriteLine($"The Item to Craft is Level {currentBestLevel}. {currentBestItem}");
                    Console.WriteLine($"The Next Skill Level is {leveltarget}");

                    if(currentBestItem != "" && currentBestItem != null)
                    {
                        var carftingInfo = await sql.GetGraftingInfo(currentBestItem);

                        int i = 1;
                        foreach(var craftitem in carftingInfo)
                        {
                            itemcraftqty += craftitem.Quantity;
                        }

                        foreach(var craftitem in carftingInfo)
                        {
                            Console.WriteLine($"{i}. Need {craftitem.Quantity} of {craftitem.Material} to make {craftitem.Item} it is at X:{craftitem.X} Y:{craftitem.Y}");
                            craftable = (int)(maxitems*(craftitem.Quantity / itemcraftqty));
                            Console.WriteLine($"The total amount of items that can be gathered are {craftable}");
                            i++;
                        }

                        Console.WriteLine(carftingInfo.Count);
                        while(level < leveltarget)
                        {

                            moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=4, y=1}, "Moving to bank:");

                            if(moveInfo != null)
                            {
                                if(moveInfo?.Character?.Inventory != null)
                                {
                                    foreach(var bankingitem in moveInfo.Character.Inventory)
                                    {
                                        if(bankingitem.Quantity > 0)
                                        {
                                            await api.PerformActionAsync<bankItemResponse>(characterName, token, "bank/deposit", new {code = bankingitem.Code, quantity = bankingitem.Quantity}, $"Banking {bankingitem.Quantity} of {bankingitem.Code}:");
                                        }
                                    }
                                }
                                
                            }
                            

                            currentTotal = 0;
                            foreach(var craftitem in carftingInfo)
                            {
                                moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move",new {x=craftitem.X, y=craftitem.Y}, $"Moving to {craftitem.Material}");
                                gathereditemMax = (int)(maxitems*(craftitem.Quantity / itemcraftqty));
                                Console.WriteLine($"Need to gather {gathereditemMax}");
                                gathereditems = 0;
                                gatheredcraftable = 0;
                                while(gathereditems < gathereditemMax && currentTotal < maxitems)
                                {
                                    
                                    gatherInfo = await api.PerformActionAsync<gatheringResponse>(characterName, token, "gathering", new{}, $"Gathering {craftitem.Material}:");

                                    if(gatherInfo?.Detail?.Items != null)
                                    {
                                        foreach(var item in gatherInfo.Detail.Items)
                                        {
                                            if(item.Quantity > 0 && item.Code == craftitem.Material)
                                            {
                                                gatheredcraftable += item.Quantity;
                                                gathereditems += item.Quantity;
                                                Console.WriteLine($"Gathered {item.Quantity} of {item.Code} the total for the item is {gatheredcraftable} and for the node {gathereditemMax}");

                                                currentTotal += item.Quantity;
                                                Console.WriteLine($"The item total is {currentTotal} of {maxitems}.");
                                            }
                                            else if (item.Quantity > 0)
                                            {
                                                gathereditems += item.Quantity;
                                                Console.WriteLine($"Gathered {item.Quantity} of {item.Code} the total for this node is {gathereditems} of {gathereditemMax}");

                                                currentTotal += item.Quantity;
                                                Console.WriteLine($"The item total is {currentTotal} of {maxitems}.");
                                            }
                                        }
                                    }

                                    Console.WriteLine($"{gathereditems} < {gathereditemMax}");
                                    Console.WriteLine($"{currentTotal} < {maxitems}");
                                    //Got to here
                                }

                                if((int)gatheredcraftable/itemcraftqty < craftable)
                                {
                                    Console.WriteLine($"Craftable was {craftable}");
                                    craftable = (int)gatheredcraftable/craftitem.Quantity;
                                    Console.WriteLine($"Craftable is now {craftable}");
                                }
                                
                            }

                            moveInfo = await api.PerformActionAsync<MoveResponse>(characterName, token, "move", new { x=1, y=5}, "Moving to crafting:"); //MANUAL ENTRY HERE

                            craftInfo = await api.PerformActionAsync<craftResponse>(characterName, token, "crafting", new { code = currentBestItem, quantity = craftable}, $"Crafting {craftable} of {currentBestItem}"); 

                            level = craftInfo?.Character?.MiningLevel; //MANUAL ENTRY HERE
                            Console.WriteLine($"Current Level is {level} trying to reach {leveltarget}");
                        }

                        Console.WriteLine("Debug Break New Level hit");
                        Console.ReadLine();
                    }
                    

                    
                }
            }
        }
    }
}