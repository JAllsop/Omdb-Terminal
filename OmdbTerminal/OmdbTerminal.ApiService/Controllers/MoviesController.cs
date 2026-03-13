using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OmdbTerminal.ApiService.Services;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    /// <summary>
    /// Searches the external OMDB database for movies similar to the specified title and returns a paginated list of results
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title, [FromQuery] int page = 1, [FromQuery] MediaType? type = null, [FromQuery] string? year = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Search title cannot be empty");
        }

        var result = await movieService.SearchAsync(title, page, type, year);
        return Ok(result);
    }

    /// <summary>
    /// Fetches the full details of a specific movie by its IMDB ID
    /// </summary>
    [HttpGet("detailsId/{id}")]
    [ProducesResponseType(typeof(MovieDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetails(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Search ID cannot be empty");
        }

        var movie = await movieService.GetDetailsByIdAsync(id);

        return movie == null
            ? NotFound($"Movie with ID {id} not found.")
            : Ok(movie);
    }

    /// <summary>
    /// Fetches the full details of a specific movie by its Title
    /// </summary>
    [HttpGet("detailsTitle/{title}")]
    [ProducesResponseType(typeof(MovieDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailsByTitle(string title, [FromQuery] MediaType? type = null, [FromQuery] string? year = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Search title cannot be empty");
        }
        var movie = await movieService.GetDetailsByTitleAsync(title, type, year);
        return movie == null
            ? NotFound($"Movie with Title {title} not found.")
            : Ok(movie);
    }
}