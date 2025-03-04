using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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

        public async Task AutoGathering(string characterName, string token)
        {
            Console.WriteLine("STARTING GATHERING TASK");
            string url = $"https://api.artifactsmmo.com/characters/{characterName}";
            var requestBody = new {};

            HttpResponseMessage response = await _client.GetAsync(url);
            var charinfo = await api.HandleGetResponse<characterInfoResponse>(response);

            //get location that will be the primary resource gathering.
            if(charinfo != null)
            {
                int? gatherX = charinfo.X, gatherY = charinfo.Y;

                //loop
                while(true)
                {
                    if(Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;

                        if(key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("Ending Loop as ESC was pressed");
                            break;
                        }
                    }

                    Console.WriteLine("Emptying Inventory");
                    await api.MoveCharacterAsync(characterName, token, 4, 1);
                    if(charinfo.Inventory != null)
                    {
                        foreach(var item in charinfo.Inventory)
                        {
                            if(item.Quantity > 0)
                            {
                                await api.BankItemsAsync(characterName, token, item.Code, item.Quantity);
                            }
                        }
                    }
                }
            }
        }
    }
}