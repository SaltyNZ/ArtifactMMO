using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Data.SQLite;
using Dapper;

namespace ArtifactMMO
{
    public class SQLiteScript
    {
        public async Task SQLiteUpdate()
        {
            await GetLocations();
            return;
        }

        private async Task GetLocations()
        {
            string apiUrl = "https://api.artifactsmmo.com/maps";

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to get Data from API");
                return;
            }

            string json = await response.Content.ReadAsStringAsync();
            MapApiResponse? apiResponse = JsonSerializer.Deserialize<MapApiResponse>(json);

            if(apiResponse is null || apiResponse.Data.Count == 0)
            {
                Console.WriteLine("No Data Returned");
                return;
            }

            List<LocationData> locations = apiResponse.Data.Select(loc => new LocationData
            {
                Name = loc.Name,
                X = loc.X,
                Y = loc.Y,
                Type = loc.Content.Type,
                Code = loc.Content.Code
            }).ToList();

            if(locations is not null)
            {

            }



        }

        private async Task SaveLocationsToDatabase(List<LocationData> locations)
        {
            string dbPath = "ArtifactDB.db";

            using SQLiteConnection conn = new(dbPath);
                conn.Execute(@"CREATE TABLE IF NOT EXISTS MAP (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    NAME TEXT,
                    X INTEGER NOT NULL,
                    Y INTEGER NOT NULL,
                    TYPE TEXT,
                    CODE TEXT,
                    UNIQUE(X, Y) -- Prevent duplicate tiles
                )");

            using var transaction = conn.BeginTransaction();
            foreach (var location in locations)
            {
                conn.Execute(
                    "INSERT OR REPLACE INTO MAP (NAME, X, Y, TYPE, CODE) VALUES (@name, @x, @y, @type, @code)", 
                    new { location.Name, location.X, location.Y, location.Type, location.Code }
                );
            }
            transaction.Commit();
            Console.WriteLine($"Stored {locations.Count} locations in the database.");
        }
    }

    //DATA CLASSES
    public class LocationData
    {
        public string? Name { get; set; } // "name"
        public int X { get; set; }        // "x"
        public int Y { get; set; }        // "y"
        public string Type { get; set; } = string.Empty; // "content.type"
        public string Code { get; set; } = string.Empty; // "content.code"
    }

    public class MapApiResponse
    {
        public List<ApiLocation> Data { get; set; } = new();
    }

    public class ApiLocation
    {
        public string? Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ApiContent Content { get; set; } = new();
    }

    public class ApiContent
    {
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

}