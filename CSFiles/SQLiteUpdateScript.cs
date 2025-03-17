using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Json;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;
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
            string apiUrlTemplate = "https://api.artifactsmmo.com/maps?page=";
            int page = 1;
            int totalPages = 1;
            List<LocationData> allLocations = new();

            using HttpClient client = new();

            do
            {
                string apiUrl = apiUrlTemplate + page;
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if(!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to get Data from API");
                    break;
                }

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response (Raw JSON): {json}");
                MapApiResponse? apiResponse = JsonSerializer.Deserialize<MapApiResponse>(json);

                if(apiResponse is null || apiResponse.Data.Count == 0)
                {
                    Console.WriteLine($"No Data Returned on Page {page}, Data Count = {apiResponse?.Data.Count}, APIURL = {apiUrl}");
                    break;
                }

                totalPages = apiResponse.Pages;

                List<LocationData> locations = apiResponse.Data.Select(loc => new LocationData
                {
                    Name = loc.Name,
                    X = loc.X,
                    Y = loc.Y,
                    Type = loc.Content?.Type ?? "none",
                    Code = loc.Content?.Code ?? "none"
                }).ToList();

                allLocations.AddRange(locations);
                page++;

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
            string connectionString = $"Data Source={dbPath};Version=3;";

            using var conn = new SQLiteConnection(connectionString);

            await conn.OpenAsync();

            await conn.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS MAP (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NAME TEXT,
                X INTEGER NOT NULL,
                Y INTEGER NOT NULL,
                TYPE TEXT,
                CODE TEXT,
                UNIQUE(X, Y) -- Prevent duplicate tiles
            )");

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                foreach (var location in locations)
                {
                    await conn.ExecuteAsync(
                        "INSERT OR REPLACE INTO MAP (NAME, X, Y, TYPE, CODE) VALUES (@Name, @X, @Y, @Type, @Code)",
                        new { location.Name, location.X, location.Y, location.Type, location.Code },
                        transaction
                    );
                }

                await transaction.CommitAsync();
                Console.WriteLine($"Stored {locations.Count} locations in the database.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error saving locations: {ex.Message}");
            }
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
        [JsonPropertyName("data")]
        public List<ApiLocation> Data { get; set; } = new();
        
        [JsonPropertyName("total")]
        public int Total { get; set; }  // Total number of map tiles available

        [JsonPropertyName("page")]
        public int Page { get; set; }   // Current page number

        [JsonPropertyName("size")]
        public int Size { get; set; }   // Number of items per page

        [JsonPropertyName("pages")]
        public int Pages { get; set; }  // Total pages available
    }

    public class ApiLocation
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("content")]
        public ApiContent? Content { get; set; }
    }

    public class ApiContent
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string? Code { get; set; } = string.Empty;
    }

}