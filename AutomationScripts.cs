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


        private async Task Gathering(string characterName, string token)
        {
            Console.WriteLine("STARTING GATHERING TASK");
            await api.CharacterInfoAsync(characterName, token);
        }
    }
}