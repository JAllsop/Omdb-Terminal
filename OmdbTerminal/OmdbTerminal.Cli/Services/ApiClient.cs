using OmdbTerminal.Shared;
using System.Net.Http.Json;
namespace OmdbTerminal.Cli.Services
{
    internal class ApiClient(MoviesHttpClient moviesClient, CachedEntriesHttpClient cachedEntriesClient) : IApiClient
    {
        public async Task SearchAndDisplayAsync(string title)
        {
            try
            {
                Console.WriteLine($"\n Searching OMDB for '{title}'...");

                var response = await moviesClient.GetFromJsonAsync<OmdbSearchResponse>($"search?title={title}");

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

                var response = await moviesClient.GetFromJsonAsync<MovieDetails>($"details/{id}");
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
                var id = Guid.NewGuid().ToString();
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
            try
            {
                Console.WriteLine("\n--- Search Cached Movies (OData) ---");
                Console.WriteLine("Example queries:");
                Console.WriteLine("  ?$filter=contains(Title, 'Star')");
                Console.WriteLine("  ?$orderby=Year desc");
                Console.WriteLine("  ?$top=5");
                Console.Write("\nEnter OData query (press Enter for all): ");

                var query = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(query) && !query.StartsWith('?'))
                {
                    query = "?" + query;
                }

                var res = await cachedEntriesClient.GetAsync(query);
                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n Failed to search movies: {msg}\n");
                    return;
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
