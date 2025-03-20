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
            await GetResourceInfo();

            return;
        }










        #region Get Map Data

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
                //Console.WriteLine($"API Response (Raw JSON): {json}"); //Debuging line
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
            
            //Clear Table
            await conn.ExecuteAsync("DELETE FROM MAP;");
            await conn.ExecuteAsync("DELETE FROM sqlite_sequence WHERE name='MAP';");

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
                await conn.CloseAsync();            
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error saving locations: {ex.Message}");
                await conn.CloseAsync();
            }
        }

        #endregion





        #region Get Resource Info
        private async Task GetResourceInfo()
        {
            string apiUrlTemplate = "https://api.artifactsmmo.com/resources?page=";
            int page = 1;
            int totalPages = 1;
            List<ResourceHDRData> allResourceHDR = new();
            List<ResourceLineData> allResourceLine = new();

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
                //Console.WriteLine($"API Response (Raw JSON): {json}"); //Debuging line
                ResourceAPIResponse? apiResponse = JsonSerializer.Deserialize<ResourceAPIResponse>(json);

                if(apiResponse is null || apiResponse.Data.Count == 0)
                {
                    Console.WriteLine($"No Data Returned on Page {page}, Data Count = {apiResponse?.Data.Count}, APIURL = {apiUrl}");
                    break;
                }

                totalPages = apiResponse.Pages;

                List<ResourceHDRData> resourceHDR = apiResponse.Data.Select(HDR => new ResourceHDRData
                {
                    Name = HDR.Name,
                    Code = HDR.Code,
                    Skill = HDR.Skill,
                    Level = HDR.Level
                }).ToList();

                allResourceHDR.AddRange(resourceHDR);

                foreach(var resource in apiResponse.Data)
                {
                    if (resource.Drops != null)
                    {
                        foreach (var drop in resource.Drops)
                        {
                            allResourceLine.Add(new ResourceLineData
                            {
                                HDR_Code = resource.Code,
                                Code = drop.Code,
                                Rate = drop.Rate,
                                Min_Qty = drop.Min_QTY,
                                Max_Qty = drop.Max_QTY
                            });
                        }
                    }
                }
                
                page++;

            } while(page <= totalPages);

            if(allResourceHDR.Any())
            {
                await SaveResourceToDatabase(allResourceHDR, allResourceLine).ConfigureAwait(false);
            }
            else
            {
                Console.WriteLine("No Loaction Found.");
            }
        }

        private async Task SaveResourceToDatabase(List<ResourceHDRData> resourceHDR, List<ResourceLineData> resourceLine)
        {
            string dbPath = "ArtifactDB.db";
            string connectionString = $"Data Source={dbPath};Version=3;";

            using var conn = new SQLiteConnection(connectionString);

            await conn.OpenAsync().ConfigureAwait(false);

            await conn.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS RESOURCE_HDR (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    NAME TEXT,
                    CODE TEXT UNIQUE, -- Ensures unique resource codes
                    SKILL TEXT,
                    LEVEL INTEGER
                );
                CREATE TABLE IF NOT EXISTS RESOURCE_LINES (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    HDR_CODE TEXT,
                    CODE TEXT,
                    RATE INTEGER,
                    MIN_QTY INTEGER,
                    MAX_QTY INTEGER
                );
            ").ConfigureAwait(false);
            
            //Clear Table
            await conn.ExecuteAsync("DELETE FROM RESOURCE_HDR;").ConfigureAwait(false);
            await conn.ExecuteAsync("DELETE FROM RESOURCE_LINES;").ConfigureAwait(false);

            using var transaction = await conn.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                if (resourceHDR.Count > 0)
                {
                    foreach(var hdr in resourceHDR)
                    {
                        await conn.ExecuteAsync(
                            "INSERT INTO RESOURCE_HDR (NAME, CODE, SKILL, LEVEL) VALUES (@Name, @Code, @Skill, @Level);",
                            new {hdr.Name, hdr.Code, hdr.Skill, hdr.Level},
                            transaction
                        ).ConfigureAwait(false);
                    }
                }


                if (resourceLine.Count > 0)
                {
                    foreach(var line in resourceLine)
                    {
                        await conn.ExecuteAsync(
                        "INSERT INTO RESOURCE_LINES (HDR_CODE, CODE, RATE, MIN_QTY, MAX_QTY) VALUES (@HDR_Code, @Code, @Rate, @Min_Qty, @Max_Qty);",
                        new {line.HDR_Code, line.Code, line.Rate, line.Min_Qty, line.Max_Qty},
                        transaction
                        ).ConfigureAwait(false);
                    }
                    
                }

                await transaction.CommitAsync().ConfigureAwait(false);
                Console.WriteLine($"Stored {resourceHDR.Count} resources headers and {resourceLine.Count} resource lines in the database.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                Console.WriteLine($"Error saving locations: {ex.Message}");
            }
            finally
            {
                await conn.CloseAsync().ConfigureAwait(false);
            }

        }

        #endregion
    }

    //DATA CLASSES
    public class LocationData
    {
        public string? Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class MapApiResponse
    {
        [JsonPropertyName("data")]
        public List<ApiLocation> Data { get; set; } = new();
        
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }
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

    public class ResourceHDRData
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Skill { get; set; }
        public int? Level { get; set; }

    }

    public class ResourceLineData
    {
        public string? HDR_Code { get; set; }
        public string? Code { get; set; }
        public int? Rate { get; set; }
        public int? Min_Qty { get; set; }
        public int? Max_Qty { get; set; }

    }

    public class ResourceAPIResponse
    {
        [JsonPropertyName("data")]
        public List<ApiResource> Data { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }
    }

    public class ApiResource
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("skill")]
        public string? Skill { get; set; }
        
        [JsonPropertyName("level")]
        public int? Level { get; set; }

        [JsonPropertyName("drops")]
        public List<ResourceDrops>? Drops { get; set; }

    }

    public class ResourceDrops
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        
        [JsonPropertyName("rate")]
        public int? Rate { get; set; }

        [JsonPropertyName("min_quantity")]
        public int? Min_QTY { get; set; }

        [JsonPropertyName("max_quantity")]
        public int? Max_QTY { get; set; }
    }
}