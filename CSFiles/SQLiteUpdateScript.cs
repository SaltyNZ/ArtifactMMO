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
            int page = 1;
            int totalPages = 1;
            List<LocationData> allLocations = new();

            using HttpClient client = new();

            do
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if(!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to get Data from API");
                    break;
                }

                string json = await response.Content.ReadAsStringAsync();
                MapApiResponse? apiResponse = JsonSerializer.Deserialize<MapApiResponse>(json);

                if(apiResponse is null || apiResponse.Data.Count == 0)
                {
                    Console.WriteLine($"No Data Returned on Page {page}");
                    break;
                }

                totalPages = apiResponse.Pages;

                List<LocationData> locations = apiResponse.Data.Select(loc => new LocationData
                {
                    Name = loc.Name,
                    X = loc.X,
                    Y = loc.Y,
                    Type = loc.Content.Type,
                    Code = loc.Content.Code
                }).ToList();

            } while(page <= totalPages);

            if(allLocations.Any())
            {
                await SaveLocationsToDatabase(allLocations).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine("No Loaction Found.");
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
        public int Total { get; set; }  // Total number of map tiles available
        public int Page { get; set; }   // Current page number
        public int Size { get; set; }   // Number of items per page
        public int Pages { get; set; }  // Total pages available
    }

    public class ApiLocation
    {
        public string? Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ApiContent Content { get; set; } = new();
        public int Pages { get; set; }
    }

    public class ApiContent
    {
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

}