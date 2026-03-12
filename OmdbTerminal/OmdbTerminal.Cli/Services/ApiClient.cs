using OmdbTerminal.Shared;
using System.Net.Http.Json;
namespace OmdbTerminal.Cli.Services
{
    internal class ApiClient(HttpClient httpClient) : IApiClient
    {
        public async Task SearchAndDisplayAsync(string title)
        {
            try
            {
                Console.WriteLine($"\n Searching OMDB for '{title}'...");

                var response = await httpClient.GetFromJsonAsync<OmdbSearchResponse>($"movies/search?title={title}");

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
                var response = await httpClient.GetFromJsonAsync<MovieDetails>($"movies/details/{id}");
                if (response == null)
                {
                    Console.WriteLine("\n Movie not found or API error\n");
                    return;
                }
                Console.WriteLine($"\n--- Details for '{response.Title}' ---");
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
    }
}
