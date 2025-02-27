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
            Console.WriteLine("1. Move");
            Console.WriteLine("2. Attack Current Square");
            Console.WriteLine("3. Rest");
            Console.WriteLine("4. Gathering");
            Console.WriteLine("5. Unequip");
            Console.WriteLine("6. Crafting");
            Console.WriteLine("0. Exit");
        }

        public bool isValidEquipment(string equipment)
        {
            foreach(string i in validEquipment)
            {
                if(equipment.ToLower() == i) return true;
                else Console.WriteLine(i);   
            }
            Console.WriteLine("Didn't hit");
            return false;
        }
    }
}