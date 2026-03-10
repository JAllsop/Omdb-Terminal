using Microsoft.AspNetCore.Mvc;
using OmdbTerminal.ApiService.Services;
using OmdbTerminal.Shared;
using static System.Net.WebRequestMethods;

namespace OmdbTerminal.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController(IOmdbClient omdbClient, ILogger<MoviesController> logger) : ControllerBase
{
    /// <summary>
    /// Searches the external OMDB database for movies matching a title
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(OmdbSearchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string title, [FromQuery] int page = 1)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Search title cannot be empty");
        }

        logger.LogInformation("Searching OMDB for: {Title}, Page: {Page}", title, page);

        var result = await omdbClient.SearchMoviesAsync(title, page);
        return Ok(result);
    }

    /// <summary>
    /// Fetches the full details of a specific movie by its IMDB ID
    /// </summary>
    [HttpGet("details/{id}")]
    [ProducesResponseType(typeof(MovieDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetails(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Movie ID cannot be empty");
        }

        logger.LogInformation("Fetching OMDB details for ID: {Id}", id);

        var details = await omdbClient.GetMovieDetailsAsync(id);

        if (details.Response == "False")
        {
            return NotFound($"Movie with ID {id} not found");
        }

        return Ok(details);
    }
}