using Terminal.Gui;
using OmdbTerminal.Shared;
using OmdbTerminal.Cli.Services;

namespace OmdbTerminal.Cli.Gui;

public class CachedEntriesView : View
{
    private readonly IApiClient _apiClient;
    private readonly ListView _entriesList;
    private List<MovieDetails> _cachedMovies = [];

    private int _currentPage = 1;
    private const int _pageSize = 20;

    private readonly RadioGroup _searchModeGroup;

    // Basic search fields
    private readonly TextField _titleInput;
    private readonly TextField _yearInput;
    private readonly ComboBox _typeCombo;
    private readonly CheckBox _isDetailedCheck;
    private readonly CheckBox _isCustomCheck;

    // Custom search fields
    private readonly TextField _customOdataInput;

    private readonly TextField _pageInput;
    private readonly Label _statusLabel;

    public CachedEntriesView() : base()
    {
        _apiClient = IoC.Container.GetInstance<IApiClient>();

        // Cache Operations Controls
        var clearBtn = new Button("Clear Cache") { X = 1, Y = 0 };
        clearBtn.Clicked += async () =>
        {
            var confirm = MessageBox.Query("Confirm", "Are you sure you want to clear all cached entries?", "Yes", "No");
            if (confirm != 0) return;

            await _apiClient.ClearCacheAsync();
            await LoadEntriesAsync();
        };

        var addBtn = new Button("Add Entry") { X = Pos.Right(clearBtn) + 2, Y = 0 };
        addBtn.Clicked += async () =>
        {
            var movie = new MovieDetails() { ImdbId = Guid.NewGuid().ToString(), IsCustom = true };
            await EditEntryAsync(movie, isNew: true);
        };

        Add(clearBtn, addBtn);

        // Search Mode Selector
        _searchModeGroup = new RadioGroup(["Basic Search", "Custom OData"]) { X = 1, Y = 2, DisplayMode = DisplayModeLayout.Horizontal };
        Add(_searchModeGroup);

        // Basic Search UI Configuration
        var titleLbl = new Label("Title:") { X = 1, Y = 4 };
        _titleInput = new TextField("") { X = 8, Y = 4, Width = 20 };
        _titleInput.KeyPress += async (e) =>
        {
            if (e.KeyEvent.Key != Key.Enter) return;

            e.Handled = true;
            _currentPage = 1;
            await LoadEntriesAsync();
        };

        var yearLbl = new Label("Year:") { X = Pos.Right(_titleInput) + 2, Y = 4 };
        _yearInput = new TextField("") { X = Pos.Right(yearLbl) + 1, Y = 4, Width = 6 };

        var typeLbl = new Label("Type:") { X = Pos.Right(_yearInput) + 2, Y = 4 };
        _typeCombo = new ComboBox() { X = Pos.Right(typeLbl) + 1, Y = 4, Width = 15, Height = 5 };
        _typeCombo.SetSource(new List<string> { "Any", "Movie", "Series", "Episode" });
        _typeCombo.SelectedItem = 0;

        _isDetailedCheck = new CheckBox("Is Detailed") { X = 1, Y = 5 };
        _isCustomCheck = new CheckBox("Is Custom") { X = Pos.Right(_isDetailedCheck) + 2, Y = 5 };

        Add(titleLbl, _titleInput, yearLbl, _yearInput, typeLbl, _typeCombo, _isDetailedCheck, _isCustomCheck);

        // Custom OData Search UI Configuration
        var customLbl = new Label("OData:") { X = 1, Y = 7 };
        var customHelp = new Label("e.g. contains(tolower(Title), 'batman') and Year eq '2022'") { X = 8, Y = 8 };
        _customOdataInput = new TextField("") { X = 8, Y = 7, Width = Dim.Fill() - 2 };
        _customOdataInput.KeyPress += async (e) =>
        {
            if (e.KeyEvent.Key != Key.Enter) return;

            e.Handled = true;
            _currentPage = 1;
            await LoadEntriesAsync();
        };


        Add(customLbl, _customOdataInput, customHelp);

        // Unified Search Controls
        var searchBtn = new Button("Search") { X = 1, Y = 10 };
        searchBtn.Clicked += async () =>
        {
            _currentPage = 1;
            await LoadEntriesAsync();
        };

        _statusLabel = new Label("") { X = Pos.Right(searchBtn) + 2, Y = 10, Width = Dim.Fill() };

        Add(searchBtn, _statusLabel);

        // Entries List View
        var entriesContainer = new FrameView()
        {
            X = 1,
            Y = 12,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 3,
            Border = new Border() { BorderStyle = BorderStyle.None }
        };

        _entriesList = new ListView(new List<string>())
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            AllowsMarking = false
        };

        _entriesList.OpenSelectedItem += async (e) =>
        {
            if (e.Item >= 0 && e.Item < _cachedMovies.Count)
            {
                var movie = _cachedMovies[e.Item];
                await ShowEntryOptions(movie);
            }
        };

        entriesContainer.Add(_entriesList);

        var scrollBar = new ScrollBarView(_entriesList, true);

        scrollBar.ChangedPosition += () =>
        {
            _entriesList.TopItem = scrollBar.Position;
            if (_entriesList.TopItem != scrollBar.Position)
            {
                scrollBar.Position = _entriesList.TopItem;
            }
            _entriesList.SetNeedsDisplay();
        };

        _entriesList.DrawContent += (e) =>
        {
            scrollBar.Size = _entriesList.Source.Count;
            scrollBar.Position = _entriesList.TopItem;
            scrollBar.Refresh();
        };

        entriesContainer.Add(scrollBar);

        Add(entriesContainer);

        // Pagination Controls Development
        var prevBtn = new Button("Prev") { X = 1, Y = Pos.AnchorEnd(1) };
        prevBtn.Clicked += async () =>
        {
            if (_currentPage <= 1) return;

            _currentPage--;
            await LoadEntriesAsync();
        };

        var pageLabel = new Label("Page:") { X = Pos.Right(prevBtn) + 2, Y = Pos.AnchorEnd(1) };
        _pageInput = new TextField("1") { X = Pos.Right(pageLabel) + 1, Y = Pos.AnchorEnd(1), Width = 5 };
        var goBtn = new Button("Go") { X = Pos.Right(_pageInput) + 1, Y = Pos.AnchorEnd(1) };

        goBtn.Clicked += async () =>
        {
            if (int.TryParse(_pageInput.Text.ToString(), out int p) && p > 0)
            {
                _currentPage = p;
                await LoadEntriesAsync();
            }
        };

        var nextBtn = new Button("Next") { X = Pos.Right(goBtn) + 2, Y = Pos.AnchorEnd(1) };
        nextBtn.Clicked += async () =>
        {
            _currentPage++;
            await LoadEntriesAsync();
        };

        Add(prevBtn, pageLabel, _pageInput, goBtn, nextBtn);

        // Initial Fetch
        _ = LoadEntriesAsync();
    }

    private async Task LoadEntriesAsync()
    {
        try
        {
            Application.MainLoop.Invoke(() =>
            {
                _statusLabel.Text = "Loading...";
                _pageInput.Text = _currentPage.ToString();
                Application.Refresh();
            });

            var odataQuery = BuildODataQuery();

            // Build pagination suffix
            int skip = (_currentPage - 1) * _pageSize;
            var paging = $"$skip={skip}&$top={_pageSize}";
            var finalQuery = string.IsNullOrEmpty(odataQuery) ? $"?{paging}" : $"?{odataQuery}&{paging}";

            // Fetch records from API and update UI
            var entries = await _apiClient.GetCachedEntriesAsync(finalQuery);
            if (entries == null) return;

            _cachedMovies = entries;
            var displayList = _cachedMovies.Select(GetDisplayTextForMovie).ToList();

            Application.MainLoop.Invoke(() =>
            {
                _entriesList.SetSource(displayList);
                _statusLabel.Text = $"Loaded {entries.Count} entries for Page {_currentPage}";
                Application.Refresh();
            });
        }
        catch (Exception ex)
        {
            Application.MainLoop.Invoke(() =>
            {
                _statusLabel.Text = "Error loading cache";
                MessageBox.ErrorQuery("Error", $"Could not load cache: {ex.Message}", "OK");
            });
        }
    }

    private string BuildODataQuery()
    {
        var odataQuery = "";

        // Custom OData Mode
        if (_searchModeGroup.SelectedItem != 0)
        {
            var customQuery = _customOdataInput.Text.ToString()?.Trim();
            if (!string.IsNullOrEmpty(customQuery))
            {
                var queryParams = customQuery.Split('&', StringSplitOptions.RemoveEmptyEntries);
                var formattedParams = new List<string>();

                var odataKeywords = new[] { "filter", "expand", "orderby", "select", "top", "skip", "count" };

                for (int i = 0; i < queryParams.Length; i++)
                {
                    var p = queryParams[i];
                    var part = p.Trim();
                    var eqIndex = part.IndexOf('=');

                    bool matchedKeyword = false;

                    if (eqIndex > 0)
                    {
                        var key = part[..eqIndex].Trim();
                        var cleanKey = key.TrimStart('$').ToLower();

                        if (odataKeywords.Contains(cleanKey))
                        {
                            formattedParams.Add($"${cleanKey}={part.Substring(eqIndex + 1).Trim()}");
                            matchedKeyword = true;
                        }
                    }

                    if (!matchedKeyword)
                    {
                        // Treat the first item as a raw filter if it lacks an OData keyword prefix
                        if (i == 0 && !part.StartsWith('$') && !part.StartsWith("filter=", StringComparison.OrdinalIgnoreCase))
                        {
                            formattedParams.Add($"$filter={part}");
                        }
                        else
                        {
                            formattedParams.Add(part);
                        }
                    }
                }

                odataQuery = string.Join("&", formattedParams);
            }
            return odataQuery;
        }

        // Basic Search Mode
        var filters = new List<string>();

        var title = _titleInput.Text.ToString()?.Trim();
        if (!string.IsNullOrEmpty(title)) filters.Add($"contains(tolower(Title), '{title.ToLower()}')");

        var year = _yearInput.Text.ToString()?.Trim();
        if (!string.IsNullOrEmpty(year)) filters.Add($"Year eq '{year}'");

        if (_typeCombo.SelectedItem > 0)
        {
            var type = (MediaType)(_typeCombo.SelectedItem - 1);
            filters.Add($"Type eq '{type}'");
        }

        if (_isDetailedCheck.Checked == true) filters.Add("IsDetailed eq true");

        if (_isCustomCheck.Checked == true) filters.Add("IsCustom eq true");

        if (filters.Count != 0) odataQuery = "$filter=" + string.Join(" and ", filters);

        return odataQuery;
    }

    private async Task ShowEntryOptions(MovieDetails movie)
    {
        var result = MessageBox.Query("Options", $"Select action for {movie.Title}", "View Details", "Edit", "Delete", "Cancel");
        switch (result)
        {
            case 0: // View Details
                Application.Run(new MovieDetailsWindow(movie, updatedDetails => UpdateItemDetails(updatedDetails)));
                break;
            case 1: // Edit
                await EditEntryAsync(movie, isNew: false);
                break;
            case 2: // Delete
                await HandleDeleteAction(movie);
                break;
        }
    }

    private void UpdateItemDetails(MovieDetails updatedDetails)
    {
        var index = _cachedMovies.FindIndex(m => m.ImdbId == updatedDetails.ImdbId);
        if (index == 0) return;

        _cachedMovies[index] = updatedDetails;
        var displayList = _cachedMovies.Select(GetDisplayTextForMovie).ToList();
        Application.MainLoop.Invoke(() =>
        {
            var topItem = _entriesList.TopItem;
            var selectedItem = _entriesList.SelectedItem;

            _entriesList.SetSource(displayList);
            _entriesList.TopItem = topItem;
            _entriesList.SelectedItem = selectedItem;

            _entriesList.SetNeedsDisplay();
            Application.Refresh();
        });

    }

    private async Task HandleDeleteAction(MovieDetails movie)
    {
        var confirm = MessageBox.Query("Confirm", $"Are you sure you want to delete {movie.Title}?", "Yes", "No");
        if (confirm != 0) return;

        try
        {
            var success = await _apiClient.DeleteCachedEntryAsync(movie.ImdbId);
            Application.MainLoop.Invoke(async () =>
            {
                if (success)
                {
                    MessageBox.Query("Success", "Entry deleted.", "OK");
                    await LoadEntriesAsync();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Could not delete entry.", "OK");
                }
            });
        }
        catch (Exception ex)
        {
            Application.MainLoop.Invoke(() => MessageBox.ErrorQuery("Error", $"API Error: {ex.Message}", "OK"));
        }
    }

    private async Task EditEntryAsync(MovieDetails movie, bool isNew)
    {
        var editDialog = new Dialog(isNew ? "Add Custom Entry" : "Edit Entry")
        {
            Width = Dim.Percent(80),
            Height = Dim.Percent(80)
        };

        var scrollView = new ScrollView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 2,
            ContentSize = new Size(80, 45),
            ShowVerticalScrollIndicator = true,
            ShowHorizontalScrollIndicator = true
        };

        var layout = new View { Width = 78, Height = 45 };
        scrollView.Add(layout);

        int row = 0;

        var titleLbl = new Label("Title:") { X = 1, Y = row };
        var titleInput = new TextField(movie.Title ?? "") { X = 15, Y = row++, Width = 60 };

        var yearLbl = new Label("Year:") { X = 1, Y = row };
        var yearInput = new TextField(movie.Year ?? "") { X = 15, Y = row++, Width = 60 };

        var typeLbl = new Label("Type:") { X = 1, Y = row };
        var typeStr = movie.Type?.ToString() ?? "Movie";
        var typeIndex = typeStr == "Movie" ? 0 : (typeStr == "Series" ? 1 : 2);
        var typeGroup = new RadioGroup(["Movie", "Series", "Episode"]) { X = 15, Y = row++, SelectedItem = typeIndex };
        row += 2; // Radio group takes more space

        var ratedLbl = new Label("Rated:") { X = 1, Y = row };
        var ratedInput = new TextField(movie.Rated ?? "") { X = 15, Y = row++, Width = 60 };

        var releasedLbl = new Label("Released:") { X = 1, Y = row };
        var releasedInput = new TextField(movie.Released ?? "") { X = 15, Y = row++, Width = 60 };

        var runtimeLbl = new Label("Runtime:") { X = 1, Y = row };
        var runtimeInput = new TextField(movie.Runtime ?? "") { X = 15, Y = row++, Width = 60 };

        var genreLbl = new Label("Genre:") { X = 1, Y = row };
        var genreInput = new TextField(movie.Genre ?? "") { X = 15, Y = row++, Width = 60 };

        var directorLbl = new Label("Director:") { X = 1, Y = row };
        var directorInput = new TextField(movie.Director ?? "") { X = 15, Y = row++, Width = 60 };

        var writerLbl = new Label("Writer:") { X = 1, Y = row };
        var writerInput = new TextField(movie.Writer ?? "") { X = 15, Y = row++, Width = 60 };

        var actorsLbl = new Label("Actors:") { X = 1, Y = row };
        var actorsInput = new TextField(movie.Actors ?? "") { X = 15, Y = row++, Width = 60 };

        var plotLbl = new Label("Plot:") { X = 1, Y = row };
        var plotInput = new TextField(movie.Plot ?? "") { X = 15, Y = row++, Width = 60 };

        var languageLbl = new Label("Language:") { X = 1, Y = row };
        var languageInput = new TextField(movie.Language ?? "") { X = 15, Y = row++, Width = 60 };

        var countryLbl = new Label("Country:") { X = 1, Y = row };
        var countryInput = new TextField(movie.Country ?? "") { X = 15, Y = row++, Width = 60 };

        var awardsLbl = new Label("Awards:") { X = 1, Y = row };
        var awardsInput = new TextField(movie.Awards ?? "") { X = 15, Y = row++, Width = 60 };

        var posterLbl = new Label("Poster URL:") { X = 1, Y = row };
        var posterInput = new TextField(movie.PosterUrl ?? "") { X = 15, Y = row++, Width = 60 };

        var imdbRatingLbl = new Label("IMDB Rating:") { X = 1, Y = row };
        var imdbRatingInput = new TextField(movie.ImdbRating ?? "") { X = 15, Y = row++, Width = 60 };

        var imdbVotesLbl = new Label("IMDB Votes:") { X = 1, Y = row };
        var imdbVotesInput = new TextField(movie.ImdbVotes ?? "") { X = 15, Y = row++, Width = 60 };

        var metascoreLbl = new Label("Metascore:") { X = 1, Y = row };
        var metascoreInput = new TextField(movie.Metascore ?? "") { X = 15, Y = row++, Width = 60 };

        var boxOfficeLbl = new Label("BoxOffice:") { X = 1, Y = row };
        var boxOfficeInput = new TextField(movie.BoxOffice ?? "") { X = 15, Y = row++, Width = 60 };

        var productionLbl = new Label("Production:") { X = 1, Y = row };
        var productionInput = new TextField(movie.Production ?? "") { X = 15, Y = row++, Width = 60 };

        var websiteLbl = new Label("Website:") { X = 1, Y = row };
        var websiteInput = new TextField(movie.Website ?? "") { X = 15, Y = row++, Width = 60 };

        var dvdLbl = new Label("DVD:") { X = 1, Y = row };
        var dvdInput = new TextField(movie.DVD ?? "") { X = 15, Y = row++, Width = 60 };

        layout.Add(
            titleLbl, titleInput, yearLbl, yearInput, typeLbl, typeGroup,
            ratedLbl, ratedInput, releasedLbl, releasedInput, runtimeLbl, runtimeInput,
            genreLbl, genreInput, directorLbl, directorInput, writerLbl, writerInput,
            actorsLbl, actorsInput, plotLbl, plotInput, languageLbl, languageInput,
            countryLbl, countryInput, awardsLbl, awardsInput, posterLbl, posterInput,
            imdbRatingLbl, imdbRatingInput, imdbVotesLbl, imdbVotesInput, metascoreLbl, metascoreInput,
            boxOfficeLbl, boxOfficeInput, productionLbl, productionInput, websiteLbl, websiteInput,
            dvdLbl, dvdInput
        );

        var saveBtn = new Button("Submit", true);
        saveBtn.Clicked += async () =>
        {
            movie.Title = titleInput.Text.ToString() ?? "";
            movie.Year = yearInput.Text.ToString() ?? "";
            movie.Rated = ratedInput.Text.ToString() ?? "";
            movie.Released = releasedInput.Text.ToString() ?? "";
            movie.Runtime = runtimeInput.Text.ToString() ?? "";
            movie.Genre = genreInput.Text.ToString() ?? "";
            movie.Director = directorInput.Text.ToString() ?? "";
            movie.Writer = writerInput.Text.ToString() ?? "";
            movie.Actors = actorsInput.Text.ToString() ?? "";
            movie.Plot = plotInput.Text.ToString() ?? "";
            movie.Language = languageInput.Text.ToString() ?? "";
            movie.Country = countryInput.Text.ToString() ?? "";
            movie.Awards = awardsInput.Text.ToString() ?? "";
            movie.PosterUrl = posterInput.Text.ToString() ?? "";
            movie.ImdbRating = imdbRatingInput.Text.ToString() ?? "";
            movie.ImdbVotes = imdbVotesInput.Text.ToString() ?? "";
            movie.Metascore = metascoreInput.Text.ToString() ?? "";
            movie.BoxOffice = boxOfficeInput.Text.ToString() ?? "";
            movie.Production = productionInput.Text.ToString() ?? "";
            movie.Website = websiteInput.Text.ToString() ?? "";
            movie.DVD = dvdInput.Text.ToString() ?? "";

            var selectedTypeStr = typeGroup.SelectedItem == 0 ? "Movie" : (typeGroup.SelectedItem == 1 ? "Series" : "Episode");
            if (Enum.TryParse<MediaType>(selectedTypeStr, true, out var mType))
            {
                movie.Type = mType;
            }

            try
            {
                bool success = false;
                if (isNew)
                {
                    success = await _apiClient.AddCachedEntryAsync(movie);
                }
                else
                {
                    success = await _apiClient.UpdateCachedEntryAsync(movie.ImdbId, movie);
                }

                Application.MainLoop.Invoke(async () =>
                {
                    if (!success)
                    {
                        MessageBox.ErrorQuery("Error", "Failed to save entry.", "OK");
                        return;
                    }
                    MessageBox.Query("Success", "Entry saved.", "OK");
                    Application.RequestStop(editDialog);

                    if (isNew)
                    {
                        await LoadEntriesAsync();
                    }
                    else
                    {
                        UpdateItemDetails(movie);
                    }
                });
            }
            catch (Exception ex)
            {
                Application.MainLoop.Invoke(() => MessageBox.ErrorQuery("API Error", ex.Message, "OK"));
            }
        };

        var cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += () => Application.RequestStop(editDialog);

        editDialog.AddButton(saveBtn);
        editDialog.AddButton(cancelBtn);

        editDialog.Add(scrollView);

        Application.Run(editDialog);
    }

    private static string GetDisplayTextForMovie(MovieDetails m)
        => $"[{(m.IsCustom ? "CUST" : "API")}] {(m.IsDetailed ? "[Detailed]" : " [Basic]  ")} {m.Title} ({m.Year}) [{m.Type}] ID: {m.ImdbId}";
}