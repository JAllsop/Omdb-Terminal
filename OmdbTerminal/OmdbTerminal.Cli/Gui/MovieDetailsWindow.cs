using Terminal.Gui;
using OmdbTerminal.Shared;
using OmdbTerminal.Cli.Services;

namespace OmdbTerminal.Cli.Gui;

public class MovieDetailsWindow : Dialog
{
    private MovieDetails _movie;
    private readonly TextView _textView;
    private readonly ImageView _imageView;
    private readonly Button? _fetchFullDetailsBtn;
    private readonly Action<MovieDetails>? _onDetailsUpdated;

    public MovieDetailsWindow(MovieDetails movie, Action<MovieDetails>? onDetailsUpdated = null) : base($"Details: {(string.IsNullOrEmpty(movie.Title) ? "Unknown" : movie.Title)}")
    {
        // Window Properties Setup
        _onDetailsUpdated = onDetailsUpdated;
        Width = Dim.Percent(80);
        Height = Dim.Percent(80);
        _movie = movie;

        // Window Controls
        var closeBtn = new Button("Close");
        closeBtn.Clicked += () => Application.RequestStop(this);
        AddButton(closeBtn);

        // Optional full details fetch button for incomplete entries
        if (!_movie.IsDetailed && !_movie.IsCustom)
        {
            _fetchFullDetailsBtn = new Button("Fetch Full Details");
            _fetchFullDetailsBtn.Clicked += async () => await FetchFullDetailsAsync();
            AddButton(_fetchFullDetailsBtn);
        }

        // Main Text View
        _imageView = new ImageView()
        {
            X = 0,
            Y = 1,
            Width = Dim.Percent(35),
            Height = Dim.Fill() - 2
        };

        _textView = new TextView()
        {
            X = Pos.Right(_imageView) + 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 2,
            ReadOnly = true,
            WordWrap = true
        };

        Add(_imageView, _textView);

        UpdateDetailsView();

    }

    private void UpdateDetailsView()
    {
        // Pre-compute formatted sections for better readability
        var ratingsContext = _movie.Ratings != null && _movie.Ratings.Any()
            ? string.Join("\n", _movie.Ratings.Select(r => $"{r.Source}: {r.Value}"))
            : "None";

        var posterContext = string.IsNullOrWhiteSpace(_movie.PosterUrl) || _movie.PosterUrl == "N/A"
            ? "[Poster Placeholder - No Image Available]"
            : $"[Poster found at: {_movie.PosterUrl}]";

        // Structured text format for the detail view
        var detailsText =
$@"Title: {_movie.Title}
Year: {_movie.Year}
Rated: {_movie.Rated}
Released: {_movie.Released}
Runtime: {_movie.Runtime}
Genre: {_movie.Genre}
Director: {_movie.Director}
Writer: {_movie.Writer}
Actors: {_movie.Actors}
Language: {_movie.Language}
Country: {_movie.Country}
Awards: {_movie.Awards}
Plot: {_movie.Plot}
IMDB Rating: {_movie.ImdbRating} ({_movie.ImdbVotes} votes)
Metascore: {_movie.Metascore}
BoxOffice: {_movie.BoxOffice}
Production: {_movie.Production}
Website: {_movie.Website}
DVD: {_movie.DVD}

--- Ratings ---
{ratingsContext}

--- Poster ---
{posterContext}";

        _textView.Text = detailsText;

        if (!string.IsNullOrWhiteSpace(_movie.PosterUrl) && _movie.PosterUrl != "N/A")
        {
            _imageView.LoadImageFromUrl(_movie.PosterUrl);
        }
    }

    private async Task FetchFullDetailsAsync()
    {
        if (string.IsNullOrEmpty(_movie.ImdbId)) return;

        var apiClient = IoC.Container.GetInstance<IApiClient>();

        try
        {
            var details = await apiClient.GetMovieDetailsByIdAsync(_movie.ImdbId);
            Application.MainLoop.Invoke(() => HandleFetchDetailsSuccess(details));
        }
        catch (Exception ex)
        {
            Application.MainLoop.Invoke(() => MessageBox.ErrorQuery("API Error", ex.Message, "OK"));
        }
    }

    // Extracted logic to keep the UI invoke block cleaner
    private void HandleFetchDetailsSuccess(MovieDetails? details)
    {
        if (details != null)
        {
            _movie = details;
            _onDetailsUpdated?.Invoke(details);

            // Update UI with new details
            Title = $"Details: {_movie.Title}";
            UpdateDetailsView();

            if (_fetchFullDetailsBtn != null)
            {
                // In Terminal.Gui 1.19.0 Dialog buttons are tricky to remove. 
                // Let's hide it by disabling and clearing its text.
                _fetchFullDetailsBtn.Visible = false;
                _fetchFullDetailsBtn.Text = "";
            }

            Application.Refresh();
        }
        else
        {
            MessageBox.ErrorQuery("Error", "Could not fetch full details.", "OK");
        }
    }
}