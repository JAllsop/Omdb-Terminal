using Terminal.Gui;
using OmdbTerminal.Shared;
using OmdbTerminal.Cli.Services;

namespace OmdbTerminal.Cli.Gui;

public class MovieDetailsWindow : Dialog
{
    private MovieDetails _movie;
    private readonly TextView _textView;
    private readonly ImageView _imageView;
    private readonly Label _urlLabel;
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
            Height = Dim.Fill() - 4
        };

        _textView = new TextView()
        {
            X = Pos.Right(_imageView) + 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 4,
            ReadOnly = true,
            WordWrap = true
        };

        _urlLabel = new Label()
        {
            X = 0,
            Y = Pos.Bottom(_textView) + 1,
            Width = Dim.Fill(),
            Height = 1
        };

        Add(_imageView, _textView, _urlLabel);

        UpdateDetailsView();

    }

    private void UpdateDetailsView()
    {
        // Pre-compute formatted sections for better readability
        var ratingsContext = _movie.Ratings != null && _movie.Ratings.Any()
            ? string.Join("\n", _movie.Ratings.Select(r => $"{r.Source}: {r.Value}"))
            : "None";

        var posterContext = string.IsNullOrWhiteSpace(_movie.PosterUrl) || _movie.PosterUrl == "N/A"
            ? "No Poster Available"
            : $"Link: {_movie.PosterUrl}";

        // Structured text format for the detail view - using string builder because I don't like the formatting of interpolated multi-line strings with @
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Title: {_movie.Title}");
        sb.AppendLine($"Year: {_movie.Year}");
        sb.AppendLine($"Rated: {_movie.Rated}");
        sb.AppendLine($"Released: {_movie.Released}");
        sb.AppendLine($"Runtime: {_movie.Runtime}");
        sb.AppendLine($"Genre: {_movie.Genre}");
        sb.AppendLine($"Director: {_movie.Director}");
        sb.AppendLine($"Writer: {_movie.Writer}");
        sb.AppendLine($"Actors: {_movie.Actors}");
        sb.AppendLine($"Language: {_movie.Language}");
        sb.AppendLine($"Country: {_movie.Country}");
        sb.AppendLine($"Awards: {_movie.Awards}");
        sb.AppendLine($"Plot: {_movie.Plot}");
        sb.AppendLine($"IMDB Rating: {_movie.ImdbRating} ({_movie.ImdbVotes} votes)");
        sb.AppendLine($"Metascore: {_movie.Metascore}");
        sb.AppendLine($"BoxOffice: {_movie.BoxOffice}");
        sb.AppendLine($"Production: {_movie.Production}");
        sb.AppendLine($"Website: {_movie.Website}");
        sb.AppendLine($"DVD: {_movie.DVD}");
        sb.AppendLine();
        sb.AppendLine("--- Ratings ---");
        sb.AppendLine($"{ratingsContext}");

        _textView.Text = sb.ToString();
        _urlLabel.Text = posterContext;
        _imageView.LoadImageFromUrl(_movie.PosterUrl);
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
        if (details == null)
        {
            MessageBox.ErrorQuery("Error", "Could not fetch full details.", "OK");
            return;
        }

        _movie = details;
        _onDetailsUpdated?.Invoke(details);

        // Update UI with new details
        Title = $"Details: {_movie.Title}";
        UpdateDetailsView();

        if (_fetchFullDetailsBtn != null)
        {
            // Terminal.Gui Dialog buttons can't be/are tricky to remove - hiding by disabling and clearing its text
            _fetchFullDetailsBtn.Visible = false;
            _fetchFullDetailsBtn.Text = "";
        }

        Application.Refresh();
    }
}