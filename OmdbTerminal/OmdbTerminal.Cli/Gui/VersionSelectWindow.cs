using OmdbTerminal.Cli.Services;
using Terminal.Gui;

namespace OmdbTerminal.Cli.Gui;

public class VersionSelectWindow : Window
{
    public VersionSelectWindow() : base("OMDB Terminal - Version Select")
    {
        var label = new Label("Choose the CLI version to run:")
        {
            X = Pos.Center(),
            Y = Pos.Center() - 2
        };

        var legacyBtn = new Button("Legacy CLI (Console)")
        {
            X = Pos.Center() - 20,
            Y = Pos.Center()
        };
        legacyBtn.Clicked += () =>
        {
            Application.RequestStop();
            RunLegacyCli();
        };

        var guiBtn = new Button("New GUI (Terminal.Gui)")
        {
            X = Pos.Center() + 5,
            Y = Pos.Center()
        };
        guiBtn.Clicked += () =>
        {
            Application.RequestStop();
            Application.Run(new MainWindow());
        };

        Add(label, legacyBtn, guiBtn);
    }

    private static void RunLegacyCli()
    {
        Console.Clear();
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
                    if (!string.IsNullOrWhiteSpace(searchTitle))
                    {
                        int searchPage = int.TryParse(searchPageStr, out var parsedPage) ? parsedPage : 1;
                        Enum.TryParse<OmdbTerminal.Shared.MediaType>(searchTypeStr, true, out var searchType);
                        var st = string.IsNullOrWhiteSpace(searchTypeStr) ? null : (OmdbTerminal.Shared.MediaType?)searchType;
                        apiClient.SearchAndDisplayAsync(searchTitle, searchPage, st, searchYear).Wait();
                    }
                    break;
                case "2":
                    Console.Write("Enter IMDB ID: ");
                    var id = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        apiClient.SearchByImdbIdAndDisplayAsync(id).Wait();
                    }
                    break;
                case "3":
                    Console.Write("Enter title: ");
                    var detailTitle = Console.ReadLine();
                    Console.Write("Enter year (optional): ");
                    var detailYear = Console.ReadLine();
                    Console.Write("Enter type (Movie/Series/Episode - optional): ");
                    var detailTypeStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(detailTitle))
                    {
                        Enum.TryParse<OmdbTerminal.Shared.MediaType>(detailTypeStr, true, out var detailType);
                        var dt = string.IsNullOrWhiteSpace(detailTypeStr) ? null : (OmdbTerminal.Shared.MediaType?)detailType;
                        apiClient.SearchByTitleAndDisplayAsync(detailTitle, dt, detailYear).Wait();
                    }
                    break;
                case "4":
                    Console.Write("Are you sure you want to clear the cache? (y/n): ");
                    var confirm = Console.ReadLine();
                    if (confirm?.Trim().ToLower() == "y")
                    {
                        apiClient.ClearCacheAndDisplayAsync().Wait();
                    }
                    break;
                case "5":
                    apiClient.ManageCustomEntitiesAsync().Wait();
                    break;
                case "6":
                case "q":
                case "quit":
                    isRunning = false;
                    break;
            }
        }
    }
}