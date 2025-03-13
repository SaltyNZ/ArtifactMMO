using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;


namespace ArtifactMMO
{
    public class UI
    {
        public void CharacterInfoUI(characterInfoResponse character)
        {
            var characterTable = new Table();

            characterTable.AddColumn("Attribute").AddColumn("Value");

            characterTable.AddRow("[bold]Name[/]", character.Name ?? "N/A");
            characterTable.AddRow("[bold]HP[/]", character.Hp.ToString() ?? "N/A");
            characterTable.AddRow("[bold]Level[/]", character.Level.ToString() ?? "N/A");
            characterTable.AddRow("[bold]XP[/]", character.Xp.ToString() ?? "N/A");
            characterTable.AddRow("[bold]Gold[/]", character.Gold.ToString() ?? "N/A");
            characterTable.AddRow("[bold]Max Items[/]", character.InventoryMaxItems.ToString() ?? "N/A");

            AnsiConsole.Write(characterTable);

        }

        public void InventoryInfoUI(characterInfoResponse character)
        {
            var inventoryTable = new Table();
            
            inventoryTable.AddColumn("Slot").AddColumn("Item").AddColumn("QTY");

            if(character.Inventory != null && character.Inventory.Count > 0)
            {
                foreach(var item in character.Inventory)
                {
                    if(item.Quantity > 0)
                    {
                        inventoryTable.AddRow(
                            item.Slot.ToString(),
                            item.Code ?? "Unknown",
                            item.Quantity.ToString()
                        );
                    }
                    
                }

                AnsiConsole.Write(inventoryTable);

            }
                
        }

    }
}