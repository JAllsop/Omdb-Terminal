using Terminal.Gui;
using OmdbTerminal.Cli.Services;
using OmdbTerminal.Shared;

namespace OmdbTerminal.Cli.Gui;

public class MainWindow : Window
{
    private readonly IApiClient _apiClient;
    private readonly ListView _resultsList;
    private readonly TextField _searchInput;
    private readonly TextField _yearInput;
    private readonly ComboBox _typeCombo;
    private readonly TextField _pageInput;
    private readonly RadioGroup _searchTypeGroup;
    private readonly Label _statusLabel;

    private readonly List<ResultItem> _combinedResults = [];
    private int _currentPage = 1;
    private int _totalResults = 0;
    private string _currentQuery = "";

    public MainWindow() : base("OMDB Terminal - Main")
    {
        _apiClient = IoC.Container.GetInstance<IApiClient>();

        // Setup Main Layout
        var tabView = new TabView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var searchView = new View() { Width = Dim.Fill(), Height = Dim.Fill() };

        // Search Controls Configuration
        _searchTypeGroup = new RadioGroup(["Title", "IMDB ID"])
        {
            X = 1,
            Y = 1,
            DisplayMode = DisplayModeLayout.Horizontal
        };
        searchView.Add(_searchTypeGroup);

        var searchLabel = new Label("Search:") { X = 1, Y = 3 };
        _searchInput = new TextField("") { X = 9, Y = 3, Width = Dim.Fill() - 2 };
        _searchInput.KeyPress += async (e) =>
        {
            if (e.KeyEvent.Key != Key.Enter) return;

            e.Handled = true;
            await ExecuteSearchAsync();
        };
        searchView.Add(searchLabel, _searchInput);

        var yearLabel = new Label("Year:") { X = 1, Y = 5 };
        _yearInput = new TextField("") { X = 7, Y = 5, Width = 6 };

        var typeLabel = new Label("Type:") { X = Pos.Right(_yearInput) + 2, Y = 5 };
        _typeCombo = new ComboBox()
        {
            X = Pos.Right(typeLabel) + 1,
            Y = 5,
            Width = 15,
            Height = 5
        };
        _typeCombo.SetSource(new List<string> { "Any", "Movie", "Series", "Episode" });
        _typeCombo.SelectedItem = 0;

        var searchBtn = new Button("Search") { X = Pos.Right(_typeCombo) + 2, Y = 5 };
        searchBtn.Clicked += async () => await ExecuteSearchAsync();
        searchView.Add(yearLabel, _yearInput, typeLabel, _typeCombo, searchBtn);

        // Status Indicator
        _statusLabel = new Label("") { X = 1, Y = 7, Width = Dim.Fill(), TextAlignment = TextAlignment.Centered };
        searchView.Add(_statusLabel);

        // Results List Configuration
        var apiLabel = new Label("Results:") { X = 1, Y = 9 };
        _resultsList = new ListView(new List<string>())
        {
            X = 1,
            Y = 10,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 2,
            AllowsMarking = false
        };
        _resultsList.OpenSelectedItem += (e) =>
        {
            var item = _combinedResults[e.Item];
            Application.Run(new MovieDetailsWindow(item.Details, updatedDetails => item.Details = updatedDetails));
        };

        searchView.Add(apiLabel, _resultsList);

        // Pagination Controls Configuration
        var prevBtn = new Button("Prev") { X = 1, Y = Pos.AnchorEnd(1) };
        prevBtn.Clicked += async () =>
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await ExecuteSearchAsync(page: _currentPage);
            }
        };

        var pageLabel = new Label("Page:") { X = Pos.Right(prevBtn) + 2, Y = Pos.AnchorEnd(1) };
        _pageInput = new TextField("") { X = Pos.Right(pageLabel) + 1, Y = Pos.AnchorEnd(1), Width = 5 };
        var goBtn = new Button("Go") { X = Pos.Right(_pageInput) + 1, Y = Pos.AnchorEnd(1) };

        goBtn.Clicked += async () =>
        {
            if (int.TryParse(_pageInput.Text.ToString(), out int p) && p > 0)
            {
                var totalPages = (int)Math.Ceiling(_totalResults / 10.0);
                if (p <= totalPages || totalPages == 0) // Allow if within range, or if we don't know total pages yet
                {
                    await ExecuteSearchAsync(page: p);
                }
            }
        };

        var nextBtn = new Button("Next") { X = Pos.AnchorEnd(12), Y = Pos.AnchorEnd(1) };
        nextBtn.Clicked += async () =>
        {
            var totalPages = (int)Math.Ceiling(_totalResults / 10.0);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                await ExecuteSearchAsync(page: _currentPage);
            }
        };

        searchView.Add(prevBtn, pageLabel, _pageInput, goBtn, nextBtn);

        // Tabs Configuration
        var cachedView = new CachedEntriesView() { Width = Dim.Fill(), Height = Dim.Fill() };

        tabView.AddTab(new TabView.Tab("Search", searchView), true);
        tabView.AddTab(new TabView.Tab("Cached Entries", cachedView), false);
        Add(tabView);

        // Quit App Controls
        var quitBtn = new Button("Quit") { X = Pos.AnchorEnd(8), Y = 0 };
        quitBtn.Clicked += () =>
        {
            var result = MessageBox.Query("Quit", "Are you sure you want to quit?", "Yes", "No");
            if (result == 0)
            {
                Application.RequestStop();
            }
        };
        Add(quitBtn);
    }

    private async Task ExecuteSearchAsync(int page = 1)
    {
        var query = _searchInput.Text.ToString();
        if (string.IsNullOrWhiteSpace(query)) return;

        // Reset search state
        _currentQuery = query;
        _currentPage = page;
        _combinedResults.Clear();
        _resultsList.SetSource(new List<string>());
        _statusLabel.Text = "Searching...";
        _pageInput?.Text = _currentPage.ToString();
        Application.Refresh();

        try
        {
            var apiResultsList = new List<MovieDetails>();

            // Extract search parameters
            string? searchYear = string.IsNullOrWhiteSpace(_yearInput.Text.ToString()) ? null : _yearInput.Text.ToString();
            MediaType? searchType = _typeCombo.SelectedItem > 0 ? (MediaType)(_typeCombo.SelectedItem - 1) : null;

            // 1. Fetch and process cached entries first
            await FetchCachedEntriesAsync(query, searchYear, searchType);

            // 2. Fetch external API results
            if (_searchTypeGroup.SelectedItem == 0) // Search by Title
            {
                await FetchApiResultsByTitleAsync(query, page, searchYear, searchType, apiResultsList);
            }
            else // Search by IMDB ID
            {
                await FetchApiResultsByIdAsync(query, apiResultsList);
            }

            // 3. Exclude API results that are already in the cached results based on IMDB ID
            var cachedIds = _combinedResults.Select(c => c.ImdbId).ToHashSet();
            foreach (var r in apiResultsList)
            {
                if (!cachedIds.Contains(r.ImdbId))
                {
                    _combinedResults.Add(new ResultItem { ImdbId = r.ImdbId, DisplayText = $"[API] [{r.Type?.ToString() ?? "Unknown"}] {r.Title} ({r.Year}) ID: {r.ImdbId}", Details = r });
                }
            }

            // Update UI list view with merged results
            Application.MainLoop.Invoke(() =>
            {
                _resultsList.SetSource(_combinedResults.Select(r => r.DisplayText).ToList());
                Application.Refresh();
            });
        }
        catch (Exception ex)
        {
            HandleSearchError(ex);
        }
    }

    // Search Helper Methods
    private async Task FetchCachedEntriesAsync(string query, string? searchYear, MediaType? searchType)
    {
        string odataFilter = _searchTypeGroup.SelectedItem == 0
            ? $"contains(tolower(Title), '{query.ToLower()}') and IsCustom eq true"
            : $"Id eq '{query}' and IsCustom eq true";

        var cached = await _apiClient.GetCachedEntriesAsync(odataFilter);
        if (cached != null)
        {
            var filteredValues = cached.AsEnumerable();
            if (_searchTypeGroup.SelectedItem == 0)
            {
                if (searchYear != null) filteredValues = filteredValues.Where(m => m.Year == searchYear);
                if (searchType != null) filteredValues = filteredValues.Where(m => m.Type == searchType);
            }

            foreach (var c in filteredValues)
            {
                _combinedResults.Add(new ResultItem
                {
                    ImdbId = c.ImdbId,
                    DisplayText = $"[CUSTOM] [{c.Type?.ToString() ?? "Unknown"}] {c.Title} ({c.Year}) ID: {c.ImdbId}",
                    Details = c
                });
            }
        }
    }

    private async Task FetchApiResultsByTitleAsync(string query, int page, string? searchYear, MediaType? searchType, List<MovieDetails> apiResultsList)
    {
        var response = await _apiClient.SearchMoviesAsync(query, page, searchType, searchYear);
        MovieDetails? specificMovie = null;

        try
        {
            specificMovie = page == 1 ? await _apiClient.GetMovieDetailsByTitleAsync(query, searchType, searchYear) : null;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) { }

        if (specificMovie != null && specificMovie.ImdbId != null)
        {
            apiResultsList.Add(specificMovie);
        }

        if (response != null && response.Response && response.Results != null)
        {
            // Filter out the specific movie from paginated results if it exists
            var specificMovieId = specificMovie?.ImdbId;
            var filteredResults = response.Results
                .Where(r => r.ImdbId != specificMovieId)
                .Select(r => new MovieDetails
                {
                    ImdbId = r.ImdbId,
                    Title = r.Title,
                    Year = r.Year,
                    Type = r.Type,
                    PosterUrl = r.PosterUrl,
                    IsDetailed = false
                })
                .ToList();

            apiResultsList.AddRange(filteredResults);
            _totalResults = response.TotalResults;

            var totalPages = (int)Math.Ceiling(_totalResults / 10.0);
            Application.MainLoop.Invoke(() =>
            {
                _statusLabel.Text = $"Page {_currentPage} of {totalPages} ({_totalResults} total results)";
            });
        }
        else
        {
            Application.MainLoop.Invoke(() =>
            {
                _statusLabel.Text = "No results found.";
            });
        }
    }

    private async Task FetchApiResultsByIdAsync(string query, List<MovieDetails> apiResultsList)
    {
        MovieDetails? response = null;
        try
        {
            response = await _apiClient.GetMovieDetailsByIdAsync(query);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) { }

        if (response != null && !string.IsNullOrEmpty(response.Title))
        {
            apiResultsList.Add(response);
            Application.MainLoop.Invoke(() =>
            {
                _statusLabel.Text = "Result found by ID";
            });
        }
        else
        {
            Application.MainLoop.Invoke(() =>
            {
                _statusLabel.Text = "No results found";
            });
        }
    }

    private void HandleSearchError(Exception ex)
    {
        Application.MainLoop.Invoke(() =>
        {
            if (ex is HttpRequestException httpEx && httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _statusLabel.Text = "No results found";
                _resultsList.SetSource(_combinedResults.Select(r => r.DisplayText).ToList());
            }
            else
            {
                MessageBox.ErrorQuery("Error", $"API Error: {ex.Message}", "OK");
                _statusLabel.Text = "Error occurred.";
            }
            Application.Refresh();
        });
    }
}