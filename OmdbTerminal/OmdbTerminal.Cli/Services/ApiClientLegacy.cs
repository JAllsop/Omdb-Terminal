using OmdbTerminal.Shared;
using System.Net.Http.Json;
namespace OmdbTerminal.Cli.Services
{
    internal class ApiClientLegacy(MoviesHttpClient moviesClient, CachedEntriesHttpClient cachedEntriesClient) : IApiClientLegacy
    {
        public async Task SearchAndDisplayAsync(string title, int page = 1, MediaType? type = null, string? year = null)
        {
            try
            {
                Console.WriteLine($"\n Searching OMDB for '{title}' (Page {page})...");

                var url = $"search?title={Uri.EscapeDataString(title)}&page={page}";
                if (type.HasValue) url += $"&type={type.Value}";
                if (!string.IsNullOrWhiteSpace(year)) url += $"&year={Uri.EscapeDataString(year)}";

                var response = await moviesClient.GetFromJsonAsync<OmdbSearchResponse>(url);

                if (response == null || !response.Response || response.Results == null)
                {
                    Console.WriteLine("\n No movies found or API error\n");
                    return;
                }

                Console.WriteLine($"\n--- Results for '{title}' ---");
                foreach (var movie in response.Results)
                {
                    Console.WriteLine($"[{movie.Year}] {movie.Title} (ID: {movie.ImdbId})");
                }
                Console.WriteLine("------------------------------\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        public async Task SearchByImdbIdAndDisplayAsync(string id)
        {
            try
            {
                Console.WriteLine($"\n Fetching details for IMDB ID '{id}'...");

                var response = await moviesClient.GetFromJsonAsync<MovieDetails>($"detailsId/{id}");
                if (response == null)
                {
                    Console.WriteLine("\n Movie not found or API error\n");
                    return;
                }

                Console.WriteLine($"Title: {response.Title}");
                Console.WriteLine($"Year: {response.Year}");
                Console.WriteLine($"Rated: {response.Rated}");
                Console.WriteLine($"Released: {response.Released}");
                Console.WriteLine($"Runtime: {response.Runtime}");
                Console.WriteLine($"Genre: {response.Genre}");
                Console.WriteLine($"Director: {response.Director}");
                Console.WriteLine($"Plot: {response.Plot}");
                Console.WriteLine($"IMDB Rating: {response.ImdbRating}");
                Console.WriteLine($"Poster URL: {response.PosterUrl}");
                Console.WriteLine("--------------------------------------\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        public async Task SearchByTitleAndDisplayAsync(string title, MediaType? type = null, string? year = null)
        {
            try
            {
                Console.WriteLine($"\n Fetching details for Title '{title}'...");

                var url = $"detailsTitle/{Uri.EscapeDataString(title)}";
                var queryParams = new List<string>();
                if (type.HasValue) queryParams.Add($"type={type.Value}");
                if (!string.IsNullOrWhiteSpace(year)) queryParams.Add($"year={Uri.EscapeDataString(year)}");

                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }

                var response = await moviesClient.GetFromJsonAsync<MovieDetails>(url);
                if (response == null)
                {
                    Console.WriteLine("\n Movie not found or API error\n");
                    return;
                }

                Console.WriteLine($"Title: {response.Title}");
                Console.WriteLine($"Year: {response.Year}");
                Console.WriteLine($"Rated: {response.Rated}");
                Console.WriteLine($"Released: {response.Released}");
                Console.WriteLine($"Runtime: {response.Runtime}");
                Console.WriteLine($"Type: {response.Type}");
                Console.WriteLine($"Genre: {response.Genre}");
                Console.WriteLine($"Director: {response.Director}");
                Console.WriteLine($"Plot: {response.Plot}");
                Console.WriteLine($"IMDB Rating: {response.ImdbRating}");
                Console.WriteLine($"Poster URL: {response.PosterUrl}");
                Console.WriteLine("--------------------------------------\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        public async Task ClearCacheAndDisplayAsync()
        {
            try
            {
                Console.WriteLine("\n Clearing cache...");
                var response = await cachedEntriesClient.DeleteFromJsonAsync<ClearCacheResponse>("clear");
                if (response == null || !response.Success)
                {
                    Console.WriteLine($"\n Failed to clear cache: {response?.Message}\n");
                    return;
                }

                Console.WriteLine($"\n {response?.Message ?? "Cache cleared successfully."}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        public async Task ManageCustomEntitiesAsync()
        {
            var isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n--- Manage Custom Entities ---");
                Console.WriteLine("1. Create Custom Movie");
                Console.WriteLine("2. Get Specific Movie (from Cache)");
                Console.WriteLine("3. Update Custom Movie");
                Console.WriteLine("4. Delete Movie (from Cache)");
                Console.WriteLine("5. Search Cached Movies (OData)");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("\n> ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await CreateCustomEntityAsync();
                        break;
                    case "2":
                        await GetCachedEntityAsync();
                        break;
                    case "3":
                        await UpdateCustomEntityAsync();
                        break;
                    case "4":
                        await DeleteCachedEntityAsync();
                        break;
                    case "5":
                        await SearchCachedEntitiesAsync();
                        break;
                    case "6":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("\nInvalid option! Please try again\n");
                        break;
                }
            }
        }

        private async Task CreateCustomEntityAsync()
        {
            try
            {
                Console.WriteLine("\n--- Create Custom Movie ---");
                var id = $"CUSTOM-{Guid.NewGuid()}";
                Console.WriteLine($"Generated ID: {id}");

                Console.Write("Enter Title: ");
                var title = Console.ReadLine() ?? "";

                Console.Write("Enter Year: ");
                var year = Console.ReadLine() ?? "";

                var movie = new MovieDetails
                {
                    ImdbId = id,
                    Title = title,
                    Year = year,
                    IsCustom = true
                };

                var res = await cachedEntriesClient.PostAsJsonAsync("", movie);
                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n Failed to create movie: {msg}\n");
                    return;
                }

                Console.WriteLine("\n Custom movie created successfully!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        private async Task GetCachedEntityAsync()
        {
            try
            {
                Console.Write("\n Enter Movie ID to retrieve: ");
                var id = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(id)) return;

                var res = await cachedEntriesClient.GetAsync($"{id}");
                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine("\n Movie not found in cache.\n");
                    return;
                }

                var json = await res.Content.ReadAsStringAsync();
                Console.WriteLine($"\n Movie Data: \n {json}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        private async Task UpdateCustomEntityAsync()
        {
            try
            {
                Console.WriteLine("\n--- Update Custom Movie ---");
                Console.Write("Enter ID to update: ");
                var id = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(id)) return;

                Console.Write("Enter new Title: ");
                var title = Console.ReadLine() ?? "";

                Console.Write("Enter new Year: ");
                var year = Console.ReadLine() ?? "";

                var movie = new MovieDetails
                {
                    ImdbId = id,
                    Title = title,
                    Year = year,
                    IsCustom = true
                };

                var res = await cachedEntriesClient.PutAsJsonAsync($"{id}", movie);
                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n Failed to update movie: {msg}\n");
                }

                Console.WriteLine("\n Custom movie updated successfully!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        private async Task DeleteCachedEntityAsync()
        {
            try
            {
                Console.Write("\n Enter Movie ID to delete: ");
                var id = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(id)) return;

                var res = await cachedEntriesClient.DeleteAsync($"{id}");
                if (!res.IsSuccessStatusCode)
                {
                    Console.WriteLine("\n Failed to delete movie, or not found.\n");
                    return;
                }
                Console.WriteLine("\n Movie deleted successfully!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
            }
        }

        private async Task SearchCachedEntitiesAsync()
        {
            var isODataRunning = true;
            while(isODataRunning)
            {
                Console.WriteLine("\n--- Advanced Cached Movies Search (OData) ---");
                Console.WriteLine("1. Get All Cached Movies");
                Console.WriteLine("2. Search Title (contains)");
                Console.WriteLine("3. Filter by exact Year");
                Console.WriteLine("4. Sort (OrderBy Title or Year)");
                Console.WriteLine("5. Pagination (Top / Skip)");
                Console.WriteLine("6. Custom Raw OData Query");
                Console.WriteLine("7. Back");
                Console.Write("\n> ");

                var input = Console.ReadLine();
                var query = "";

                switch (input)
                {
                    case "1":
                        break;
                    case "2":
                        Console.Write("Enter text to search in Title: ");
                        var text = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(text)) query = $"?$filter=contains(tolower(Title), '{text.ToLower()}')";
                        break;
                    case "3":
                        Console.Write("Enter exact Year: ");
                        var year = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(year)) query = $"?$filter=Year eq '{year}'";
                        break;
                    case "4":
                        Console.Write("Order by (1 = Title, 2 = Year, default = Title): ");
                        var sortField = Console.ReadLine() == "2" ? "Year" : "Title";
                        Console.Write("Direction (1 = Ascending, 2 = Descending, default = Asc): ");
                        var sortDir = Console.ReadLine() == "2" ? "desc" : "asc";
                        query = $"?$orderby={sortField} {sortDir}";
                        break;
                    case "5":
                        Console.Write("Top (e.g. 5, default 10): ");
                        var topStr = Console.ReadLine();
                        var top = string.IsNullOrWhiteSpace(topStr) ? 10 : int.Parse(topStr);
                        Console.Write("Skip (e.g. 0, default 0): ");
                        var skipStr = Console.ReadLine();
                        var skip = string.IsNullOrWhiteSpace(skipStr) ? 0 : int.Parse(skipStr);
                        query = $"?$top={top}&$skip={skip}";
                        break;
                    case "6":
                        Console.Write("Enter raw OData query (e.g. ?$filter=contains(Title, 'Star')): ");
                        query = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(query) && !query.StartsWith('?')) query = "?" + query;
                        break;
                    case "7":
                        isODataRunning = false;
                        continue;
                    default:
                        Console.WriteLine("\nInvalid option\n");
                        continue;
                }

                try
                {
                    Console.WriteLine($"\n Executing query: GET /CachedEntries{query}");
                    var res = await cachedEntriesClient.GetAsync(query);
                    if (!res.IsSuccessStatusCode)
                    {
                        var msg = await res.Content.ReadAsStringAsync();
                        Console.WriteLine($"\n Failed to search movies: {msg}\n");
                        continue;
                    }

                    var json = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n Search Results: \n {json}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n Error communicating with API: {ex.Message}\n");
                }
            }
        }
    }
}
