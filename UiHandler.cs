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

        private string[] validEquipment = {"weapon","shield","helmet","body_armor","leg_armor","boots","ring1","ring2","amulet","artifact1","artifact2","artifact3","utility1","utility2","bag","rune"};

        public void MainUIWriteLine()
        {
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("Select a Function:");
            Console.WriteLine("1. Character Info");
            Console.WriteLine("2. Move");
            Console.WriteLine("3. Attack Current Square");
            Console.WriteLine("4. Rest");
            Console.WriteLine("5. Gathering");
            Console.WriteLine("6. Unequip");
            Console.WriteLine("7. Equip");
            Console.WriteLine("8. Crafting");
            Console.WriteLine("0. Exit");
        }

        public void CharacterInfoUI(characterInfoResponse character)
        {
            var characterTable = new Table();

            characterTable.AddColumn("Attribute").AddColumn("Value");

            characterTable.AddRow("[bold]Name[/]", character.Name ?? "N/A");
            characterTable.AddRow("[bold]HP[/]", character.Hp.ToString() ?? "N/A");
            characterTable.AddRow("[bold]Level[/]", character.Level.ToString() ?? "N/A");
            characterTable.AddRow("[bold]XP[/]", character.Xp.ToString() ?? "N/A");
            characterTable.AddRow("[bold]Gold[/]", character.Gold.ToString() ?? "N/A");

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

        public bool isValidEquipment(string equipment)
        {
            foreach(string i in validEquipment)
            {
                if(equipment.ToLower() == i) return true;  
            }
            return false;
        }

        public bool isValidCraft(string craft)
        {
            return true;
        }


    }
}