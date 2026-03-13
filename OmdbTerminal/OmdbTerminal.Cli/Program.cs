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

        var apiClient = IoC.Container.GetInstance<IApiClientLegacy>();
        var isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Search OMDB");
            Console.WriteLine("2. Get movie details by IMDB ID (and cache result)");
            Console.WriteLine("3. Get movie details by Title (and cache result)");
            Console.WriteLine("4. Clear cache");
            Console.WriteLine("5. Manage Cached Entries (CRUD)");
            Console.WriteLine("6. Quit");
            Console.Write("\n> ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Enter title: ");
                    var searchTitle = Console.ReadLine();
                    Console.Write("Enter year (optional): ");
                    var searchYear = Console.ReadLine();
                    Console.Write("Enter type (Movie/Series/Episode - optional): ");
                    var searchTypeStr = Console.ReadLine();
                    Console.Write("Enter page (optional, default 1): ");
                    var searchPageStr = Console.ReadLine();
                    int searchPage = int.TryParse(searchPageStr, out var parsedPage) ? parsedPage : 1;
                    Enum.TryParse<OmdbTerminal.Shared.MediaType>(searchTypeStr, true, out var searchType);

                    if (!string.IsNullOrWhiteSpace(searchTitle))
                    {
                        var st = string.IsNullOrWhiteSpace(searchTypeStr) ? null : (OmdbTerminal.Shared.MediaType?)searchType;
                        await apiClient.SearchAndDisplayAsync(searchTitle, searchPage, st, searchYear);
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
                    Console.Write("Enter title: ");
                    var detailTitle = Console.ReadLine();
                    Console.Write("Enter year (optional): ");
                    var detailYear = Console.ReadLine();
                    Console.Write("Enter type (Movie/Series/Episode - optional): ");
                    var detailTypeStr = Console.ReadLine();
                    Enum.TryParse<OmdbTerminal.Shared.MediaType>(detailTypeStr, true, out var detailType);

                    if (!string.IsNullOrWhiteSpace(detailTitle))
                    {
                        var dt = string.IsNullOrWhiteSpace(detailTypeStr) ? null : (OmdbTerminal.Shared.MediaType?)detailType;
                        await apiClient.SearchByTitleAndDisplayAsync(detailTitle, dt, detailYear);
                    }
                    break;
                case "4":
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
                case "5":
                    await apiClient.ManageCustomEntitiesAsync();
                    break;
                case "6":
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