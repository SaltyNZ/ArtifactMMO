using System;
using System.Net.Http;
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