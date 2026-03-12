using OmdbTerminal.Cli.Services;

namespace OmdbTerminal.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=================================================");
        Console.WriteLine(" OMDB TERMINAL - COMMAND LINE INTERFACE V1.0");
        Console.WriteLine("=================================================");
        Console.ResetColor();

        var apiClient = IoC.Container.GetInstance<IApiClient>();
        var isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Search OMDB");
            Console.WriteLine("2. Get movie details by IMDB ID (and cache result)");
            Console.WriteLine("3. Clear cache");
            Console.WriteLine("4. Quit");
            Console.Write("\n> ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Enter movie title: ");
                    var title = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        await apiClient.SearchAndDisplayAsync(title);
                    }
                    break;
                case "2":
                    Console.Write("Enter IMDB ID: ");
                    var id = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        await apiClient.SearchByImdbIdAndDisplayAsync(id);
                    }
                    break;
                case "3":
                    Console.Write("Are you sure you want to clear the cache? (y/n): ");
                    var confirm = Console.ReadLine();
                    if (confirm?.Trim().ToLower() == "y")
                    {
                        await apiClient.ClearCacheAndDisplayAsync();
                    }
                    else
                    {
                        Console.WriteLine("Cache clear cancelled\n");
                    }
                    break;
                case "4":
                case "q":
                case "quit":
                    isRunning = false;
                    Console.WriteLine("Shutting down...");
                    break;
                default:
                    Console.WriteLine("\nInvalid option! Please try again\n");
                    break;
            }
        }
    }
}