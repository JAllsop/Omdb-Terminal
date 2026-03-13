using Terminal.Gui;
using OmdbTerminal.Shared;
using OmdbTerminal.Cli.Services;

namespace OmdbTerminal.Cli.Gui;

public class MovieDetailsWindow : Dialog
{
    private MovieDetails _movie;
    private TextView _textView;
    private Button? _fetchFullDetailsBtn;
    private Action<MovieDetails>? _onDetailsUpdated;

    public MovieDetailsWindow(MovieDetails movie, Action<MovieDetails>? onDetailsUpdated = null) : base($"Details: {(string.IsNullOrEmpty(movie.Title) ? "Unknown" : movie.Title)}")
    {
        _onDetailsUpdated = onDetailsUpdated;
        Width = Dim.Percent(80);
        Height = Dim.Percent(80);
        _movie = movie;

        var closeBtn = new Button("Close");
        closeBtn.Clicked += () => Application.RequestStop(this);
        AddButton(closeBtn);

        if (!_movie.IsDetailed && !_movie.IsCustom)
        {
            _fetchFullDetailsBtn = new Button("Fetch Full Details");
            _fetchFullDetailsBtn.Clicked += async () => await FetchFullDetailsAsync();
            AddButton(_fetchFullDetailsBtn);
        }

        _textView = new TextView()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 2,
            ReadOnly = true,
            WordWrap = true
        };

        Add(_textView);

        UpdateDetailsView();
    }

    private void UpdateDetailsView()
    {
        var detailsText = $"Title: {_movie.Title}\n" +
                          $"Year: {_movie.Year}\n" +
                          $"Rated: {_movie.Rated}\n" +
                          $"Released: {_movie.Released}\n" +
                          $"Runtime: {_movie.Runtime}\n" +
                          $"Genre: {_movie.Genre}\n" +
                          $"Director: {_movie.Director}\n" +
                          $"Writer: {_movie.Writer}\n" +
                          $"Actors: {_movie.Actors}\n" +
                          $"Language: {_movie.Language}\n" +
                          $"Country: {_movie.Country}\n" +
                          $"Awards: {_movie.Awards}\n" +
                          $"Plot: {_movie.Plot}\n" +
                          $"IMDB Rating: {_movie.ImdbRating} ({_movie.ImdbVotes} votes)\n" +
                          $"Metascore: {_movie.Metascore}\n" +
                          $"BoxOffice: {_movie.BoxOffice}\n" +
                          $"Production: {_movie.Production}\n" +
                          $"Website: {_movie.Website}\n" +
                          $"DVD: {_movie.DVD}\n\n" +
                          $"--- Ratings ---\n" +
                          (_movie.Ratings != null && _movie.Ratings.Any() 
                               ? string.Join("\n", _movie.Ratings.Select(r => $"{r.Source}: {r.Value}")) 
                               : "None") + "\n\n" +
                                                 $"--- Poster ---\n" +
                                                (string.IsNullOrWhiteSpace(_movie.PosterUrl) || _movie.PosterUrl == "N/A" 
                                                 ? "[Poster Placeholder - No Image Available]" 
                                                 : $"[Poster found at: {_movie.PosterUrl}]");

                              _textView.Text = detailsText;
                          }

    private async Task FetchFullDetailsAsync()
    {
        if (string.IsNullOrEmpty(_movie.ImdbId)) return;

        var apiClient = IoC.Container.GetInstance<IApiClient>();
        try
        {
            var details = await apiClient.GetMovieDetailsByIdAsync(_movie.ImdbId);
            Application.MainLoop.Invoke(() => {
                if (details != null)
                {
                    _movie = details;
                    _onDetailsUpdated?.Invoke(details);
                    Title = $"Details: {_movie.Title}";
                    UpdateDetailsView();
                    if (_fetchFullDetailsBtn != null)
                    {
                        // In Terminal.Gui 1.19.0 Dialog buttons are tricky to remove. Let's hide it by disabling and clearing its text.
                        // Ideally we recreate the dialog or disable the button.
                        _fetchFullDetailsBtn.Visible = false;
                        _fetchFullDetailsBtn.Text = "";
                    }
                    Application.Refresh();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Could not fetch full details.", "OK");
                }
            });
        }
        catch (Exception ex)
        {
            Application.MainLoop.Invoke(() => MessageBox.ErrorQuery("API Error", ex.Message, "OK"));
        }
    }
}